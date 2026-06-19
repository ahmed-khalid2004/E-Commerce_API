namespace DomainLayer.Models.ProductModule
{
    public class Product : BaseEntity<int>
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string PictureUrl { get; set; } = null!;
        public decimal Price { get; set; }

        // Inventory — default 0 means not tracked yet
        public int StockQuantity { get; set; } = 0;

        public ProductBrand ProductBrand { get; set; } = null!;
        public int BrandId { get; set; }

        public ProductType ProductType { get; set; } = null!;
        public int TypeId { get; set; }
        public decimal? Discount { get; set; }   
        public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();

        public ICollection<ProductCategory> ProductCategories { get; set; }
            = new List<ProductCategory>();
    }
}