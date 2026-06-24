using Shared.DataTransferObjects.IdentityDTOs;

namespace ServiceAbstracion
{
    public interface IAuthenticationService
    {
        Task<UserDTO> LoginAsync(LoginDTO loginDTO);
        Task<UserDTO> RegisterAsync(RegisterDTO registerDTO);
        Task<bool> CheckEmailAsync(string email);
        Task<UserDTO> GetCurrentUserAsync(string email);
        Task<AddressDTO> GetCurrentUserAddressAsync(string email);
        Task<AddressDTO> UpdateCurrentUserAddressAsync(string email, AddressDTO addressDTO);

        Task SendResetCodeAsync(string email);
        Task<string?> VerifyResetCodeAsync(string email, string code);

        Task ResetPasswordAsync(ResetPasswordDTO dto);
    }
}