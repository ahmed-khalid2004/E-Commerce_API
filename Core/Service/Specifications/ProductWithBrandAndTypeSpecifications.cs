using DomainLayer.Models.ProductModule;
using Shared;

namespace Service.Specifications
{
    class ProductWithBrandAndTypeSpecifications : BaseSpecifications<Product, int>
    {
        public ProductWithBrandAndTypeSpecifications(ProductQueryParams queryParams)
            : base(p =>
                (!queryParams.BrandId.HasValue || p.BrandId == queryParams.BrandId) &&
                (!queryParams.SubCategoryId.HasValue || p.SubCategoryId == queryParams.SubCategoryId) &&
                (!queryParams.CategoryId.HasValue || p.SubCategory.CategoryId == queryParams.CategoryId) &&
                (string.IsNullOrWhiteSpace(queryParams.search) || p.Name.ToLower().Contains(queryParams.search.ToLower()))
            )
        {
            AddInclude(p => p.ProductBrand);

            // SubCategory + its parent Category in one chained include
            AddInclude("SubCategory.Category");

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
            AddInclude("SubCategory.Category");
        }
    }
}