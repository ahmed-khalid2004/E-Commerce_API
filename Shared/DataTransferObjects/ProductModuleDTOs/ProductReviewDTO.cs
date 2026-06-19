namespace Shared.DataTransferObjects.ProductModuleDTOs
{
    public class ProductReviewDTO
    {
        public int Id { get; set; }
        public string UserDisplayName { get; set; } = default!;
        public string Comment { get; set; } = default!;
        public int? Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<ProductReviewDTO> Replies { get; set; } = [];
    }
}