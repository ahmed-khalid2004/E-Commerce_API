using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.ProductModuleDTOs
{
    public class CreateSubCategoryDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }
    }
}