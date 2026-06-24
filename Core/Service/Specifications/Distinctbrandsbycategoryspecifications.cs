using DomainLayer.Models.ProductModule;

namespace Service.Specifications
{
    /// <summary>
    /// Returns ProductBrand rows that have at least one Product whose
    /// SubCategory belongs to the given parent Category — i.e. brands
    /// present anywhere under that Category, across all its SubCategories.
    /// </summary>
    class DistinctBrandsByCategorySpecifications : BaseSpecifications<ProductBrand, int>
    {
        public DistinctBrandsByCategorySpecifications(int categoryId)
            : base(b => b.Products.Any(p => p.SubCategory.CategoryId == categoryId))
        {
            AddOrderBy(b => b.Name);
        }
    }
}