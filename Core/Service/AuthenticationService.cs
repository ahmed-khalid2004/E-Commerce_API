using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.IdentityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServiceAbstracion;
using Shared.DataTransferObjects.IdentityDTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Service
{
    public class AuthenticationService(
        UserManager<ApplicationUser> _userManager,
        IConfiguration _configuration,
        IMapper _mapper,
        IEmailService _emailService,
        IRedisClient _redisClient) : IAuthenticationService
    {
        // ── Auth ──────────────────────────────────────────────────────────────

        public async Task<UserDTO> LoginAsync(LoginDTO loginDTO)
        {
            var user = await _userManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Email == loginDTO.Email)
                ?? throw new UserNotFoundException(loginDTO.Email);

            if (!await _userManager.CheckPasswordAsync(user, loginDTO.Password))
                throw new UnauthorizedException();

            return await BuildUserDtoAsync(user);
        }

        public async Task<UserDTO> RegisterAsync(RegisterDTO registerDTO)
        {
            var user = new ApplicationUser
            {
                DisplayName = registerDTO.DisplayName,
                Email = registerDTO.Email,
                UserName = registerDTO.Email,
                PhoneNumber = registerDTO.PhoneNumber
            };

            if (registerDTO.Address is not null)
                user.Address = _mapper.Map<AddressDTO, Address>(registerDTO.Address);

            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(e => e.Description).ToList());

            return await BuildUserDtoAsync(user);
        }

        public async Task<bool> CheckEmailAsync(string email)
            => await _userManager.FindByEmailAsync(email) is not null;

        public async Task<UserDTO> GetCurrentUserAsync(string email)
        {
            var user = await _userManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Email == email)
                ?? throw new UserNotFoundException(email);

            return await BuildUserDtoAsync(user);
        }

        // ── Address ───────────────────────────────────────────────────────────

        public async Task<AddressDTO> GetCurrentUserAddressAsync(string email)
        {
            var user = await _userManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Email == email)
                ?? throw new UserNotFoundException(email);

            return _mapper.Map<Address, AddressDTO>(user.Address);
        }

        public async Task<AddressDTO> UpdateCurrentUserAddressAsync(string email, AddressDTO dto)
        {
            var user = await _userManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Email == email)
                ?? throw new UserNotFoundException(email);

            if (user.Address is not null)
            {
                user.Address.FirstName = dto.FirstName;
                user.Address.LastName = dto.LastName;
                user.Address.Street = dto.Street;
                user.Address.City = dto.City;
                user.Address.Country = dto.Country;
            }
            else
            {
                user.Address = _mapper.Map<AddressDTO, Address>(dto);
            }

            await _userManager.UpdateAsync(user);
            return _mapper.Map<AddressDTO>(user.Address);
        }

        // ── Forgot Password — Step 1: Send OTP ────────────────────────────────

        public async Task SendResetCodeAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email)
                ?? throw new UserNotFoundException(email);

            var otp = new Random().Next(100000, 999999).ToString();

            // Key: "otp:{email}" — 10 minute TTL, one active OTP per email
            await _redisClient.SetAsync($"otp:{email}", otp, TimeSpan.FromMinutes(10));
            await _emailService.SendOtpAsync(email, otp);
        }

        // ── Forgot Password — Step 2: Verify OTP, issue a Reset Token ─────────
        //
        // On success, generates a random, unguessable token stored in Redis
        // keyed by email. The frontend only asks the user for the OTP ONCE
        // here — the returned token is what's sent to ResetPasswordAsync,
        // not the original 6-digit code. This closes the gap where anyone
        // who merely knows the email (without ever seeing the OTP) could
        // otherwise call reset-password directly.

        public async Task<string?> VerifyResetCodeAsync(string email, string code)
        {
            var stored = await _redisClient.GetAsync($"otp:{email}");
            if (stored is null || stored != code)
                return null;

            // OTP confirmed — consume it (one-time use) and issue a reset token
            await _redisClient.DeleteAsync($"otp:{email}");

            var resetToken = Guid.NewGuid().ToString("N"); // 32-char random token
            await _redisClient.SetAsync($"reset-token:{email}", resetToken, TimeSpan.FromMinutes(10));

            return resetToken;
        }

        // ── Forgot Password — Step 3: Reset Password using the Reset Token ────

        public async Task ResetPasswordAsync(ResetPasswordDTO dto)
        {
            var storedToken = await _redisClient.GetAsync($"reset-token:{dto.Email}");
            if (storedToken is null || storedToken != dto.ResetToken)
                throw new BadRequestException(["Invalid or expired reset session. Please request a new code."]);

            var user = await _userManager.FindByEmailAsync(dto.Email)
                ?? throw new UserNotFoundException(dto.Email);

            var identityToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, identityToken, dto.NewPassword);

            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(e => e.Description).ToList());

            // One-time use — invalidate immediately after success
            await _redisClient.DeleteAsync($"reset-token:{dto.Email}");
        }

        // ── Shared DTO builder ────────────────────────────────────────────────

        private async Task<UserDTO> BuildUserDtoAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            return new UserDTO
            {
                Id = user.Id,
                Email = user.Email!,
                DisplayName = user.DisplayName,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,
                Roles = roles,
                Address = user.Address is null ? null : _mapper.Map<Address, AddressDTO>(user.Address),
                Token = await CreateTokenAsync(user, roles)
            };
        }

        // ── Token ─────────────────────────────────────────────────────────────

        private async Task<string> CreateTokenAsync(ApplicationUser user, IList<string>? roles = null)
        {
            roles ??= await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id!),
                new(ClaimTypes.Email,          user.Email!),
                new(ClaimTypes.Name,           user.UserName!)
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var jwtSection = _configuration.GetSection("JWT:Options");
            var key = new SymmetricSecurityKey(
                                 Encoding.UTF8.GetBytes(jwtSection["SecretKey"]!));

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}