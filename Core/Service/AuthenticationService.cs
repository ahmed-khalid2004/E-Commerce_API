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
        IRedisClient  _redis) : IAuthenticationService
    {

        // ── Auth ──────────────────────────────────────────────────────────────

        public async Task<UserDTO> LoginAsync(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email)
                ?? throw new UserNotFoundException(loginDTO.Email);

            if (!await _userManager.CheckPasswordAsync(user, loginDTO.Password))
                throw new UnauthorizedException();

            return new UserDTO
            {
                Email = user.Email!,
                DisplayName = user.DisplayName,
                Token = await CreateTokenAsync(user)
            };
        }

        public async Task<UserDTO> RegisterAsync(RegisterDTO registerDTO)
        {
            var user = new ApplicationUser
            {
                DisplayName = registerDTO.DisplayName,
                Email = registerDTO.Email,
                UserName = registerDTO.Email,
                PhoneNumber = registerDTO.PhoneNumber,
                Address = _mapper.Map<AddressDTO, Address>(registerDTO.Address)  
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(e => e.Description).ToList());

            return new UserDTO
            {
                Email = user.Email!,
                DisplayName = user.DisplayName,
                Token = await CreateTokenAsync(user)
            };
        }

        public async Task<bool> CheckEmailAsync(string email)
            => await _userManager.FindByEmailAsync(email) is not null;

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

        public async Task SendResetCodeAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email)
                ?? throw new UserNotFoundException(email);

            var otp = new Random().Next(100000, 999999).ToString();
            await _redis.SetAsync($"otp:{email}", otp, TimeSpan.FromMinutes(10));
            await _emailService.SendOtpAsync(email, otp);
        }

        public async Task<bool> VerifyResetCodeAsync(string email, string code)
        {
            var stored = await _redis.GetAsync($"otp:{email}");
            if (stored is null || stored != code) return false;

            await _redis.SetAsync($"otp-verified:{email}", "true", TimeSpan.FromMinutes(10));
            await _redis.DeleteAsync($"otp:{email}");
            return true;
        }

        public async Task ResetPasswordAsync(ResetPasswordDTO dto)
        {
            var verified = await _redis.GetAsync($"otp-verified:{dto.Email}");
            if (verified != "true")
                throw new BadRequestException(["Please verify your reset code first."]);

            var user = await _userManager.FindByEmailAsync(dto.Email)
                ?? throw new UserNotFoundException(dto.Email);

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, dto.NewPassword);

            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(e => e.Description).ToList());

            await _redis.DeleteAsync($"otp-verified:{dto.Email}");
        }

        // ── Token ─────────────────────────────────────────────────────────────

        private async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id!),
                new(ClaimTypes.Email,          user.Email!),
                new(ClaimTypes.Name,           user.UserName!),
                new("DisplayName",             user.DisplayName)
            };

            var roles = await _userManager.GetRolesAsync(user);
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