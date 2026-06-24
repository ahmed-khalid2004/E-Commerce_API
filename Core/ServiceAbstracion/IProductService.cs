using Shared;
using Shared.DataTransferObjects.ProductModuleDTOs;

namespace DomainLayer.Contracts
{
    public interface IProductService
    {
        Task<PaginatedResult<ProductDTO>> GetAllProductsAsync(ProductQueryParams queryParams);
        Task<ProductDTO?> GetProductByIdAsync(int id);

        Task<IEnumerable<BrandDTO>> GetAllBrandsAsync();
        Task<BrandDTO> GetBrandByIdAsync(int id);

        Task<ProductDTO> CreateProductAsync(CreateProductDTO dto);
        Task UpdateProductAsync(int id, UpdateProductDTO dto);
        Task DeleteProductAsync(int id);

        Task<BrandDTO> CreateBrandAsync(CreateBrandDTO dto);
        Task UpdateBrandAsync(int id, CreateBrandDTO dto);
        Task DeleteBrandAsync(int id);

        // ── Derived filter data (no schema relation — inferred from Products) ──

        /// <summary>
        /// Returns the distinct Brands that actually have products under the
        /// given SubCategory. No Brand<->SubCategory relation exists in the
        /// schema by design — this is computed live from Products.
        /// </summary>
        Task<IEnumerable<BrandDTO>> GetBrandsBySubCategoryAsync(int subCategoryId);

        /// <summary>
        /// Returns the distinct Brands that have products anywhere under the
        /// given parent Category (across all its SubCategories).
        /// </summary>
        Task<IEnumerable<BrandDTO>> GetBrandsByCategoryAsync(int categoryId);
    }
}