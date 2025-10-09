using AutoMapper;
using DomainLayer.Exceptions;
using DomainLayer.Models.IdentityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServiceAbstracion;
using Shared.DataTransferObjects.IdentityDTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AuthenticationService(
        UserManager<ApplicationUser> _userManager,
        IConfiguration _configuration,
        IMapper _mapper) : IAuthenticationService
    {
        public async Task<bool> CheckEmailAsync(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            return user is not null;
        }
        public async Task<UserDTO> GetCurrentUserAsync(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email) ?? throw new UserNotFoundException(Email);
            return new UserDTO() { Email = user.Email, DisplayName = user.DisplayName, Token = await CreateTokenAsync(user)};
        }

        public async Task<AddressDTO> GetCurrentUserAddressAsync(string Email)
        {
            var user = await _userManager.Users.Include(U => U.Address)
                .FirstOrDefaultAsync(U => U.Email==Email) ?? throw new UserNotFoundException(Email);
            if (user.Address is not null) return _mapper.Map<Address,AddressDTO>(user.Address);
            else
                throw new AddressNotFoundException(user.UserName);
        }
        public async Task<AddressDTO> UpdateCurrentUserAddressAsync(string Email, AddressDTO addressDTO)
        {
            var user = await _userManager.Users.Include(U => U.Address)
              .FirstOrDefaultAsync(U => U.Email == Email) ?? throw new UserNotFoundException(Email);
            if (user.Address is not null)
            {
                user.Address.FirstName = addressDTO.FirstName;
                user.Address.LastName = addressDTO.LastName;
                user.Address.Street = addressDTO.Street;
                user.Address.City = addressDTO.City;
                user.Address.Country = addressDTO.Country;
            }
            else
            {
                user.Address = _mapper.Map<AddressDTO, Address>(addressDTO);
            }
            await _userManager.UpdateAsync(user);
            return _mapper.Map<AddressDTO>(user.Address);
        }

        public async Task<UserDTO> LoginAsync(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email) ?? throw new UserNotFoundException(loginDTO.Email);

            var isValid = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (isValid)
                return new UserDTO() { Email = user.Email, DisplayName = user.DisplayName, Token = await CreateTokenAsync(user) };
            else throw new UnauthorizedException();
        }

        public async Task<UserDTO> RegisterAsync(RegisterDTO registerDTO)
        {
            var user = new ApplicationUser()
            {
                DisplayName = registerDTO.DisplayName,
                Email = registerDTO.Email,
                UserName = registerDTO.Email,
                PhoneNumber = registerDTO.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (result.Succeeded)
                return new UserDTO() { Email = user.Email, DisplayName = user.DisplayName, Token = await CreateTokenAsync(user) };
            else
            {
                var Errors = result.Errors.Select(e => e.Description).ToList();
                throw new BadRequestException(Errors);
            }
        }


        private async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName!)
            };
            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var jwtSection = _configuration.GetSection("JWT:Options");
            var secretKey = jwtSection["SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1), 
                signingCredentials: creds 
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
