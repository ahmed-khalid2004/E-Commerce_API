using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstraction;
using Shared.DataTransferObjects.IdentityDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    public class AuthenticationController(IServiceManager _serviceManager) : ApiBaseController
    {
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login( LoginDTO loginDTO)
        {
            var user = await _serviceManager.AuthenticationService.LoginAsync(loginDTO);
            return Ok(user);
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register( RegisterDTO registerDTO)
        {
            var user = await _serviceManager.AuthenticationService.RegisterAsync(registerDTO);
            return Ok(user);
        }
        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmail(string email)
        {
            var exists = await _serviceManager.AuthenticationService.CheckEmailAsync(email);
            return Ok(exists);
        }
        [Authorize]
        [HttpGet("CurrentUser")]
        public async Task<ActionResult<UserDTO>> GetCurrentUser()
        {
            var user = await _serviceManager.AuthenticationService.GetCurrentUserAsync(GetEmailFromToken());
            return Ok(user);
        }
        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDTO>> GetCurrentUserAddress()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var address = await _serviceManager.AuthenticationService.GetCurrentUserAddressAsync(email);
            return Ok(address);
        }
        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDTO>> UpdateCurrentUserAddress(AddressDTO addressDTO)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var address = await _serviceManager.AuthenticationService.UpdateCurrentUserAddressAsync(email, addressDTO);
            return Ok(address);
        }
    }
}
