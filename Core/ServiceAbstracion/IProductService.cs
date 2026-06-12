using Shared;
using Shared.DataTransferObjects.ProductModuleDTOs;

namespace DomainLayer.Contracts
{
    public interface IProductService
    {
        // ── Read ──────────────────────────────────────────────────────────────
        Task<PaginatedResult<ProductDTO>> GetAllProductsAsync(ProductQueryParams queryParams);
        Task<ProductDTO?> GetProductByIdAsync(int id);
        Task<IEnumerable<BrandDTO>> GetAllBrandsAsync();
        Task<BrandDTO> GetBrandByIdAsync(int id);
        Task<IEnumerable<TypeDTO>> GetAllTypesAsync();
        Task<TypeDTO> GetTypeByIdAsync(int id);

        // ── Products Admin CRUD ───────────────────────────────────────────────
        Task<ProductDTO> CreateProductAsync(CreateProductDTO dto);
        Task UpdateProductAsync(int id, UpdateProductDTO dto);
        Task DeleteProductAsync(int id);

        // ── Brands Admin CRUD ─────────────────────────────────────────────────
        Task<BrandDTO> CreateBrandAsync(CreateBrandDTO dto);
        Task UpdateBrandAsync(int id, CreateBrandDTO dto);
        Task DeleteBrandAsync(int id);

        // ── Types Admin CRUD ──────────────────────────────────────────────────
        Task<TypeDTO> CreateTypeAsync(CreateTypeDTO dto);
        Task UpdateTypeAsync(int id, CreateTypeDTO dto);
        Task DeleteTypeAsync(int id);
    }
}