using Shared.DataTransferObjects.IdentityDTOs;

namespace ServiceAbstracion
{
    public interface IAuthenticationService
    {
        Task<UserDTO> LoginAsync(LoginDTO loginDTO);
        Task<UserDTO> RegisterAsync(RegisterDTO registerDTO);
        Task<bool> CheckEmailAsync(string email);
        Task<AddressDTO> GetCurrentUserAddressAsync(string email);
        Task<AddressDTO> UpdateCurrentUserAddressAsync(string email, AddressDTO addressDTO);

        // Forgot Password — 3 steps
        Task SendResetCodeAsync(string email);          // Step 1: generate OTP + send email
        Task<bool> VerifyResetCodeAsync(string email, string code); // Step 2: verify OTP
        Task ResetPasswordAsync(ResetPasswordDTO dto);  // Step 3: reset with OTP + new password
    }
}