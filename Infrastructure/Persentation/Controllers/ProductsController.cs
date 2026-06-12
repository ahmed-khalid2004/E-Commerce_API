using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Attributes;
using ServicesAbstraction;
using Shared;
using Shared.DataTransferObjects.ProductModuleDTOs;

namespace Presentation.Controllers
{
    public class ProductsController(IServiceManager _serviceManager) : ApiBaseController
    {
        [AllowAnonymous]
        [HttpGet]
        [Cache]
        public async Task<ActionResult<PaginatedResult<ProductDTO>>> GetAllProducts(
            [FromQuery] ProductQueryParams queryParams)
            => Ok(await _serviceManager.ProductService.GetAllProductsAsync(queryParams));

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        [Cache]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
            => Ok(await _serviceManager.ProductService.GetProductByIdAsync(id));

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [InvalidateCache("api/products")]
        public async Task<ActionResult<ProductDTO>> CreateProduct([FromBody] CreateProductDTO dto)
        {
            var created = await _serviceManager.ProductService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        [InvalidateCache("api/products")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDTO dto)
        {
            await _serviceManager.ProductService.UpdateProductAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        [InvalidateCache("api/products")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _serviceManager.ProductService.DeleteProductAsync(id);
            return NoContent();
        }
    }
}