using Shared.DataTransferObjects.IdentityDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstracion
{
    public interface IAuthenticationService
    {
        Task<UserDTO> LoginAsync(LoginDTO loginDTO);
        Task<UserDTO> RegisterAsync(RegisterDTO registerDTO);

        Task<bool> CheckEmailAsync(string Email);
        Task<UserDTO> GetCurrentUserAsync(string Email);

        Task<AddressDTO> GetCurrentUserAddressAsync(string Email);


        Task<AddressDTO> UpdateCurrentUserAddressAsync(string Email, AddressDTO addressDTO);
    }
}
