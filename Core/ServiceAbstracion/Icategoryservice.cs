using Shared.DataTransferObjects.ProductModuleDTOs;

namespace ServiceAbstracion
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<CategoryDTO> GetCategoryByIdAsync(int id);
        Task<CategoryDTO> CreateCategoryAsync(CreateCategoryDTO dto);
        Task UpdateCategoryAsync(int id, CreateCategoryDTO dto);
        Task DeleteCategoryAsync(int id);
    }
}