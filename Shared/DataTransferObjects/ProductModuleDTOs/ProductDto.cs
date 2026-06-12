namespace Shared.DataTransferObjects.ProductModuleDTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PictureUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ProductBrand { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty;
        // Phase 4+: category names; empty list when product has no categories
        public List<string> Categories { get; set; } = [];
    }
}