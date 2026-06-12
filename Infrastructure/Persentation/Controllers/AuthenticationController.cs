using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstraction;
using Shared.DataTransferObjects.IdentityDTOs;
using System.Security.Claims;

namespace Presentation.Controllers
{
    public class AuthenticationController(IServiceManager _serviceManager) : ApiBaseController
    {
        // Public — no token needed to log in
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await _serviceManager.AuthenticationService.LoginAsync(loginDTO);
            return Ok(user);
        }

        // Public — no token needed to register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            var user = await _serviceManager.AuthenticationService.RegisterAsync(registerDTO);
            return Ok(user);
        }

        // Public — used during registration form validation
        [AllowAnonymous]
        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmail(string email)
        {
            var exists = await _serviceManager.AuthenticationService.CheckEmailAsync(email);
            return Ok(exists);
        }

        // Authenticated — needs valid token
        [Authorize]
        [HttpGet("CurrentUser")]
        public async Task<ActionResult<UserDTO>> GetCurrentUser()
        {
            var user = await _serviceManager.AuthenticationService.GetCurrentUserAsync(GetEmailFromToken());
            return Ok(user);
        }

        // Authenticated — user's own address
        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDTO>> GetCurrentUserAddress()
        {
            var address = await _serviceManager.AuthenticationService
                .GetCurrentUserAddressAsync(GetEmailFromToken());
            return Ok(address);
        }

        // Authenticated — user updates their own address
        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDTO>> UpdateCurrentUserAddress(AddressDTO addressDTO)
        {
            var address = await _serviceManager.AuthenticationService
                .UpdateCurrentUserAddressAsync(GetEmailFromToken(), addressDTO);
            return Ok(address);
        }
    }
}