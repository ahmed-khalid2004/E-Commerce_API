namespace DomainLayer.Models.ProductModule
{
    public class Category : BaseEntity<int>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        // 1-to-many: a Category owns many SubCategories.
        // Replaces the old many-to-many ProductCategory join table.
        public ICollection<SubCategory> SubCategories { get; set; }
            = new List<SubCategory>();
    }
}