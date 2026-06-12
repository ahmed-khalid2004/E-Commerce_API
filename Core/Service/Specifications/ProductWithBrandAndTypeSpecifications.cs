using DomainLayer.Models.ProductModule;
using Shared;

namespace Service.Specifications
{
    class ProductWithBrandAndTypeSpecifications : BaseSpecifications<Product, int>
    {
        public ProductWithBrandAndTypeSpecifications(ProductQueryParams queryParams)
            : base(p =>
                (!queryParams.BrandId.HasValue || p.BrandId == queryParams.BrandId) &&
                (!queryParams.TypeId.HasValue || p.TypeId == queryParams.TypeId) &&
                (!queryParams.CategoryId.HasValue || p.ProductCategories.Any(pc => pc.CategoryId == queryParams.CategoryId)) &&
                (string.IsNullOrWhiteSpace(queryParams.search) || p.Name.ToLower().Contains(queryParams.search.ToLower()))
            )
        {
            AddInclude(p => p.ProductBrand);
            AddInclude(p => p.ProductType);
            // Loads ProductCategories join rows + their Category in one SQL JOIN
            AddInclude("ProductCategories.Category");

            switch (queryParams.sort)
            {
                case ProductSortingOption.PriceAsc:
                    AddOrderBy(p => p.Price);
                    break;
                case ProductSortingOption.PriceDesc:
                    AddOrderByDescending(p => p.Price);
                    break;
                default:
                    AddOrderBy(p => p.Name);
                    break;
            }

            ApplyPagination(queryParams.PageSize, queryParams.PageNumber);
        }

        public ProductWithBrandAndTypeSpecifications(int id)
            : base(p => p.Id == id)
        {
            AddInclude(p => p.ProductBrand);
            AddInclude(p => p.ProductType);
            AddInclude("ProductCategories.Category");
        }
    }
}