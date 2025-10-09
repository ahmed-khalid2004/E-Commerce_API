using Shared;
using Shared.DataTransferObjects.ProductModuleDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{
    public interface IProductService
    {
        Task<IEnumerable<BrandDTO>> GetAllBrandsAsync();
        Task<PaginatedResult<ProductDTO>> GetAllProductsAsync(ProductQueryParams queryParams);
        Task<IEnumerable<TypeDTO>> GetAllTypesAsync();
        Task<ProductDTO?> GetProductByIdAsync(int id);
    }
}