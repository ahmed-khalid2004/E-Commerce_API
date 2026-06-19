namespace DomainLayer.Models.ProductModule
{
    public class ProductReview : BaseEntity<int>
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string UserId { get; set; } = default!;          
        public string UserDisplayName { get; set; } = default!; 

        public string Comment { get; set; } = default!;
        public int? Rating { get; set; }  

        public int? ParentReviewId { get; set; }
        public ProductReview? ParentReview { get; set; }
        public ICollection<ProductReview> Replies { get; set; } = [];

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}