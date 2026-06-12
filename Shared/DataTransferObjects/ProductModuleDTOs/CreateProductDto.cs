using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.ProductModuleDTOs
{
    public class CreateProductDTO
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string PictureUrl { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        public int TypeId { get; set; }

        // Optional — product can exist without categories
        public List<int> CategoryIds { get; set; } = [];
    }
}