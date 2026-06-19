using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.IdentityDTOs
{
    public class VerifyResetCodeDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Code must be 6 digits")]
        public string Code { get; set; } = string.Empty;
    }
}