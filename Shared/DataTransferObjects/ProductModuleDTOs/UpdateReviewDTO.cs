using System.ComponentModel.DataAnnotations;
namespace Shared.DataTransferObjects.ProductModuleDTOs
{
    public class UpdateReviewDTO
    {
        [Required, MaxLength(1000)]
        public string Comment { get; set; } = default!;
        [Range(1, 5)]
        public int? Rating { get; set; }
    }
}