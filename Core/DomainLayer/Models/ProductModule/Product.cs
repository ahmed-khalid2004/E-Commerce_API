namespace DomainLayer.Models.ProductModule
{
    public class Product : BaseEntity<int>
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string PictureUrl { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal Discount { get; set; }

        public int StockQuantity { get; set; } = 0;

        public ProductBrand ProductBrand { get; set; } = null!;
        public int BrandId { get; set; }

        // Was ProductType/TypeId — now SubCategory/SubCategoryId.
        // SubCategory belongs to a Category, so Product -> SubCategory -> Category
        // is the full chain. No more direct/many-to-many Category link on Product.
        public SubCategory SubCategory { get; set; } = null!;
        public int SubCategoryId { get; set; }

        public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
    }
}