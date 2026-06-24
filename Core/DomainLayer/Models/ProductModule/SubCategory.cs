namespace DomainLayer.Models.ProductModule
{
    /// <summary>
    /// Was "ProductType" — renamed to reflect its real role: a SubCategory
    /// that belongs to exactly one parent Category (e.g. Electronics -> Mobile).
    /// </summary>
    public class SubCategory : BaseEntity<int>
    {
        public string Name { get; set; } = default!;

        public int CategoryId { get; set; }      // FK
        public Category Category { get; set; } = null!;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}