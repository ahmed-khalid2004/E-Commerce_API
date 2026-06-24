using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.IdentityDTOs
{
    public class RegisterDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;

        [Required]
        public string DisplayName { get; set; } = default!;

        [Phone]
        public string? PhoneNumber { get; set; }

        public AddressDTO? Address { get; set; }
    }
}