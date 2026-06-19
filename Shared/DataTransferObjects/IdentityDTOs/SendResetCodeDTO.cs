using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.IdentityDTOs
{
    public class SendResetCodeDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}