namespace Shared.DataTransferObjects.ProductModuleDTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PictureUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int StockQuantity { get; set; }
        public bool InStock => StockQuantity > 0;

        public string ProductBrand { get; set; } = string.Empty;

        // Was ProductType — now SubCategory, with its parent CategoryId/CategoryName
        // so the frontend can build breadcrumbs without an extra call.
        public string SubCategory { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}