using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.IdentityDTOs
{
    public class RegisterDTO
    {
        [EmailAddress]
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        [Phone]
        public string? PhoneNumber { get; set; }

        [Required]
        public AddressDTO Address { get; set; } = default!;
    }
}