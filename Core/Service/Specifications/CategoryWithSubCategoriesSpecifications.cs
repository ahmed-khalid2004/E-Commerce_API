using DomainLayer.Models.ProductModule;

namespace Service.Specifications
{
    class CategoryWithSubCategoriesSpecifications : BaseSpecifications<Category, int>
    {
        // All categories with their subcategories — for GetAll
        public CategoryWithSubCategoriesSpecifications()
            : base(null)
        {
            AddInclude(c => c.SubCategories);
        }

        // Single category with its subcategories — for GetById
        public CategoryWithSubCategoriesSpecifications(int id)
            : base(c => c.Id == id)
        {
            AddInclude(c => c.SubCategories);
        }
    }
}