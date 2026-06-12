namespace DomainLayer.Models.ProductModule
{
    /// <summary>
    /// Pure join entity — no domain logic.
    /// Owns the M:N relationship between Product and Category.
    /// Product.cs is NOT touched; this entity carries the relationship.
    /// </summary>
    public class ProductCategory
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }

        // Navigation properties — no lazy loading; explicit Include only
        public Product Product { get; set; } = null!;
        public Category Category { get; set; } = null!;
    }
}