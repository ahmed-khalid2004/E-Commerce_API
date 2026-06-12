using DomainLayer.Models.ProductModule;
using Shared;

namespace Service.Specifications
{
    class ProductCountSpecifications : BaseSpecifications<Product, int>
    {
        public ProductCountSpecifications(ProductQueryParams queryParams)
            : base(p =>
                (!queryParams.BrandId.HasValue || p.BrandId == queryParams.BrandId) &&
                (!queryParams.TypeId.HasValue || p.TypeId == queryParams.TypeId) &&
                (!queryParams.CategoryId.HasValue || p.ProductCategories.Any(pc => pc.CategoryId == queryParams.CategoryId)) &&
                (string.IsNullOrWhiteSpace(queryParams.search) || p.Name.ToLower().Contains(queryParams.search.ToLower()))
            )
        {
            // No includes needed — EF translates .Any() into EXISTS subquery for COUNT
        }
    }
}