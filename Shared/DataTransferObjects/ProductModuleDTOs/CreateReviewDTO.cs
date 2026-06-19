using System.ComponentModel.DataAnnotations;
namespace Shared.DataTransferObjects.ProductModuleDTOs
{
    public class CreateReviewDTO
    {
        [Required]
        public int ProductId { get; set; }
        [Required, MaxLength(1000)]
        public string Comment { get; set; } = default!;
        [Required, Range(1, 5)]
        public int Rating { get; set; }
    }
}