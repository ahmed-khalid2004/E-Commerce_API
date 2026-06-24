namespace Shared.DataTransferObjects.ProductModuleDTOs
{
    // Was TypeDTO — renamed to reflect the new domain model
    public class SubCategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}