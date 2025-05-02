using Shared.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{
    public interface IProductService
    {
        Task<IEnumerable<BrandDTO>> GetAllBrandsAsync();
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
        Task<IEnumerable<TypeDTO>> GetAllTypesAsync();
        Task<ProductDTO?> GetProductByIdAsync(int id);
    }
}