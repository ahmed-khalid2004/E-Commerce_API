using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstraction;
using Shared.DataTransferObjects.IdentityDTOs;

namespace Presentation.Controllers
{
    public class AuthenticationController(IServiceManager _serviceManager) : ApiBaseController
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
            => Ok(await _serviceManager.AuthenticationService.LoginAsync(loginDTO));

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
            => Ok(await _serviceManager.AuthenticationService.RegisterAsync(registerDTO));

        [AllowAnonymous]
        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmail(string email)
            => Ok(await _serviceManager.AuthenticationService.CheckEmailAsync(email));

        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDTO>> GetCurrentUserAddress()
            => Ok(await _serviceManager.AuthenticationService.GetCurrentUserAddressAsync(GetEmailFromToken()));

        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDTO>> UpdateCurrentUserAddress(AddressDTO addressDTO)
            => Ok(await _serviceManager.AuthenticationService.UpdateCurrentUserAddressAsync(GetEmailFromToken(), addressDTO));

        // ── Forgot Password — Step 1 ──────────────────────────────────────────
        // Generates a 6-digit OTP and sends it to the user's email
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] SendResetCodeDTO dto)
        {
            await _serviceManager.AuthenticationService.SendResetCodeAsync(dto.Email);
            return Ok(new { message = "Verification code sent to your email." });
        }

        // ── Forgot Password — Step 2 ──────────────────────────────────────────
        // Verifies the OTP before showing the new password form
        [AllowAnonymous]
        [HttpPost("verify-reset-code")]
        public async Task<ActionResult<bool>> VerifyResetCode([FromBody] VerifyResetCodeDTO dto)
        {
            var isValid = await _serviceManager.AuthenticationService
                .VerifyResetCodeAsync(dto.Email, dto.Code);

            if (!isValid)
                return BadRequest(new { message = "Invalid or expired verification code." });

            return Ok(new { message = "Code verified successfully." });
        }

        // ── Forgot Password — Step 3 ──────────────────────────────────────────
        // Resets the password using the verified OTP
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            await _serviceManager.AuthenticationService.ResetPasswordAsync(dto);
            return Ok(new { message = "Password reset successfully." });
        }
    }
}