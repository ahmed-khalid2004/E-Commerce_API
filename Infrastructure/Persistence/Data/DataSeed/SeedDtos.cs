namespace Persistence.Data.DataSeed
{
    public class ProductSeedDto
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string PictureUrl { get; set; } = default!;
        public decimal Price { get; set; }
        public string BrandName { get; set; } = default!;
        // Was TypeName — now points at the SubCategory by name
        public string SubCategoryName { get; set; } = default!;
    }

    // New — replaces ProductCategorySeedDto (no more many-to-many link table)
    public class SubCategorySeedDto
    {
        public string Name { get; set; } = default!;
        public string CategoryName { get; set; } = default!;
    }
}