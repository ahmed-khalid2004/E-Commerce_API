using DomainLayer.Models.ProductModule;
using Shared;

namespace Service.Specifications
{
    class ProductCountSpecifications : BaseSpecifications<Product, int>
    {
        public ProductCountSpecifications(ProductQueryParams queryParams)
            : base(p =>
                (!queryParams.BrandId.HasValue || p.BrandId == queryParams.BrandId) &&
                (!queryParams.SubCategoryId.HasValue || p.SubCategoryId == queryParams.SubCategoryId) &&
                (!queryParams.CategoryId.HasValue || p.SubCategory.CategoryId == queryParams.CategoryId) &&
                (string.IsNullOrWhiteSpace(queryParams.search) || p.Name.ToLower().Contains(queryParams.search.ToLower()))
            )
        {
            // No includes needed — EF translates p.SubCategory.CategoryId into a JOIN
            // automatically for the WHERE clause, even without explicit Include.
        }
    }
}