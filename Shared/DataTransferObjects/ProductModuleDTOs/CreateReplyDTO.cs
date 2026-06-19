using System.ComponentModel.DataAnnotations;
namespace Shared.DataTransferObjects.ProductModuleDTOs
{
    public class CreateReplyDTO
    {
        [Required]
        public int ParentReviewId { get; set; }
        [Required, MaxLength(1000)]
        public string Comment { get; set; } = default!;
    }
}