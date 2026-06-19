using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Attributes;
using ServicesAbstraction;
using Shared.DataTransferObjects.ProductModuleDTOs;

namespace Presentation.Controllers
{
    public class BrandsController(IServiceManager _serviceManager) : ApiBaseController
    {
        [AllowAnonymous]
        [HttpGet]
        [Cache]
        public async Task<ActionResult<IEnumerable<BrandDTO>>> GetAllBrands()
            => Ok(await _serviceManager.ProductService.GetAllBrandsAsync());

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        [Cache]
        public async Task<ActionResult<BrandDTO>> GetBrand(int id)
            => Ok(await _serviceManager.ProductService.GetBrandByIdAsync(id));

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [InvalidateCache("api/brands", "api/products")]
        public async Task<ActionResult<BrandDTO>> CreateBrand([FromBody] CreateBrandDTO dto)
        {
            var created = await _serviceManager.ProductService.CreateBrandAsync(dto);
            return CreatedAtAction(nameof(GetBrand), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        [InvalidateCache("api/brands", "api/products")]
        public async Task<IActionResult> UpdateBrand(int id, [FromBody] CreateBrandDTO dto)
        {
            await _serviceManager.ProductService.UpdateBrandAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        [InvalidateCache("api/brands", "api/products")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            await _serviceManager.ProductService.DeleteBrandAsync(id);
            return NoContent();
        }
    }
}