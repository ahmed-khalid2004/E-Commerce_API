namespace Persistence.Data.DataSeed
{
    /// <summary>
    /// Internal DTO used only during seeding to deserialize products.json.
    /// Uses BrandName / TypeName strings so the seed file has no dependency
    /// on database-generated integer IDs. Never exposed outside this assembly.
    /// </summary>
    internal sealed class ProductSeedDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string PictureUrl { get; set; } = null!;
        public decimal Price { get; set; }

        /// <summary>Matches <see cref="ProductBrand.Name"/> — resolved at seed time.</summary>
        public string BrandName { get; set; } = null!;

        /// <summary>Matches <see cref="ProductType.Name"/> — resolved at seed time.</summary>
        public string TypeName { get; set; } = null!;
    }

    /// <summary>
    /// Internal DTO used only during seeding to deserialize product_categories.json.
    /// Both sides are resolved by Name to avoid any ID drift.
    /// </summary>
    internal sealed class ProductCategorySeedDto
    {
        public string ProductName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
    }
}