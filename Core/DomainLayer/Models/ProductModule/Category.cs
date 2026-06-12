namespace DomainLayer.Models.ProductModule
{
    public class Category : BaseEntity<int>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        // Navigation — optional, loaded only when explicitly included
        public ICollection<ProductCategory> ProductCategories { get; set; }
            = new List<ProductCategory>();
    }
}