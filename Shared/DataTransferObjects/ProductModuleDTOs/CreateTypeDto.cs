using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.ProductModuleDTOs
{
    public class CreateTypeDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}