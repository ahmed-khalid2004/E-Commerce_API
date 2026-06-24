namespace Shared.DataTransferObjects.ProductModuleDTOs
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Nested subcategories — this is the "tree" shape the frontend asked for
        public List<SubCategoryDTO> SubCategories { get; set; } = [];
    }
}