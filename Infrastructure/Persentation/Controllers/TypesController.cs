using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Attributes;
using ServicesAbstraction;
using Shared.DataTransferObjects.ProductModuleDTOs;

namespace Presentation.Controllers
{
    public class TypesController(IServiceManager _serviceManager) : ApiBaseController
    {
        [AllowAnonymous]
        [HttpGet]
        [Cache]
        public async Task<ActionResult<IEnumerable<TypeDTO>>> GetAllTypes()
            => Ok(await _serviceManager.ProductService.GetAllTypesAsync());

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        [Cache]
        public async Task<ActionResult<TypeDTO>> GetType(int id)
            => Ok(await _serviceManager.ProductService.GetTypeByIdAsync(id));

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [InvalidateCache("api/types", "api/products")]
        public async Task<ActionResult<TypeDTO>> CreateType([FromBody] CreateTypeDTO dto)
        {
            var created = await _serviceManager.ProductService.CreateTypeAsync(dto);
            return CreatedAtAction(nameof(GetType), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        [InvalidateCache("api/types", "api/products")]
        public async Task<IActionResult> UpdateType(int id, [FromBody] CreateTypeDTO dto)
        {
            await _serviceManager.ProductService.UpdateTypeAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        [InvalidateCache("api/types", "api/products")]
        public async Task<IActionResult> DeleteType(int id)
        {
            await _serviceManager.ProductService.DeleteTypeAsync(id);
            return NoContent();
        }
    }
}