using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Attributes;
using ServicesAbstraction;
using Shared;
using Shared.DataTransferObjects.ProductModuleDTOs;

namespace Presentation.Controllers
{
    // BaseUrl: http://localhost:5239/api/Products
    public class ProductsController(IServiceManager _serviceManager) : ApiBaseController
    {
        // GET: api/Products
        [Authorize("Admin")]
        [HttpGet]
        [Cache]
        public async Task<ActionResult<PaginatedResult<ProductDTO>>> GetAllProducts([FromQuery] ProductQueryParams queryParams)
        {
            var products = await _serviceManager.ProductService.GetAllProductsAsync(queryParams);
            return Ok(products);
        }

        // GET: api/Products/10
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await _serviceManager.ProductService.GetProductByIdAsync(id);
            return Ok(product);
        }

        [HttpGet("Types")]
        [Cache]
        public async Task<ActionResult<IEnumerable<TypeDTO>>> GetAllTypes()
        {
            var Types = await _serviceManager.ProductService.GetAllTypesAsync();
            return Ok(Types);
        }

        [HttpGet("Brands")]
        [Cache]
        public async Task<ActionResult<IEnumerable<TypeDTO>>> GetAllBrands()
        {
            var Brands = await _serviceManager.ProductService.GetAllBrandsAsync();
            return Ok(Brands);
        }
    }
}