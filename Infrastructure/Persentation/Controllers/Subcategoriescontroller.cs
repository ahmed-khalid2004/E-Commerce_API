using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Attributes;
using ServicesAbstraction;
using Shared.DataTransferObjects.ProductModuleDTOs;

namespace Presentation.Controllers
{
    public class SubCategoriesController(IServiceManager _serviceManager) : ApiBaseController
    {
        [AllowAnonymous]
        [HttpGet]
        [Cache]
        public async Task<ActionResult<IEnumerable<SubCategoryDTO>>> GetAllSubCategories()
            => Ok(await _serviceManager.CategoryService.GetAllSubCategoriesAsync());

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        [Cache]
        public async Task<ActionResult<SubCategoryDTO>> GetSubCategory(int id)
            => Ok(await _serviceManager.CategoryService.GetSubCategoryByIdAsync(id));

        // Derived filter data — the Brands that actually have products
        // under this SubCategory. No schema relation; inferred from Products.
        [AllowAnonymous]
        [HttpGet("{id:int}/brands")]
        [Cache]
        public async Task<ActionResult<IEnumerable<BrandDTO>>> GetBrandsForSubCategory(int id)
            => Ok(await _serviceManager.ProductService.GetBrandsBySubCategoryAsync(id));

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [InvalidateCache("api/subcategories", "api/categories", "api/products")]
        public async Task<ActionResult<SubCategoryDTO>> CreateSubCategory([FromBody] CreateSubCategoryDTO dto)
        {
            var created = await _serviceManager.CategoryService.CreateSubCategoryAsync(dto);
            return CreatedAtAction(nameof(GetSubCategory), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        [InvalidateCache("api/subcategories", "api/categories", "api/products")]
        public async Task<IActionResult> UpdateSubCategory(int id, [FromBody] CreateSubCategoryDTO dto)
        {
            await _serviceManager.CategoryService.UpdateSubCategoryAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        [InvalidateCache("api/subcategories", "api/categories", "api/products")]
        public async Task<IActionResult> DeleteSubCategory(int id)
        {
            await _serviceManager.CategoryService.DeleteSubCategoryAsync(id);
            return NoContent();
        }
    }
}