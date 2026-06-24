using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Attributes;
using ServicesAbstraction;
using Shared.DataTransferObjects.ProductModuleDTOs;

namespace Presentation.Controllers
{
    public class CategoriesController(IServiceManager _serviceManager) : ApiBaseController
    {
        [AllowAnonymous]
        [HttpGet]
        [Cache]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAllCategories()
            => Ok(await _serviceManager.CategoryService.GetAllCategoriesAsync());

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        [Cache]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
            => Ok(await _serviceManager.CategoryService.GetCategoryByIdAsync(id));

        // Derived filter data — Brands present anywhere under this Category
        // (across all its SubCategories). Inferred from Products, no schema change.
        [AllowAnonymous]
        [HttpGet("{id:int}/brands")]
        [Cache]
        public async Task<ActionResult<IEnumerable<BrandDTO>>> GetBrandsForCategory(int id)
            => Ok(await _serviceManager.ProductService.GetBrandsByCategoryAsync(id));

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [InvalidateCache("api/categories", "api/products", "api/subcategories")]
        public async Task<ActionResult<CategoryDTO>> CreateCategory([FromBody] CreateCategoryDTO dto)
        {
            var created = await _serviceManager.CategoryService.CreateCategoryAsync(dto);
            return CreatedAtAction(nameof(GetCategory), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        [InvalidateCache("api/categories", "api/products", "api/subcategories")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryDTO dto)
        {
            await _serviceManager.CategoryService.UpdateCategoryAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        [InvalidateCache("api/categories", "api/products", "api/subcategories")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _serviceManager.CategoryService.DeleteCategoryAsync(id);
            return NoContent();
        }
    }
}