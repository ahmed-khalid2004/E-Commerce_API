using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.IdentityDTOs
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}