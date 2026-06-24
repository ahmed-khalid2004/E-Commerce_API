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

        // SubCategory CRUD — lives here since SubCategory belongs to Category
        Task<SubCategoryDTO> CreateSubCategoryAsync(CreateSubCategoryDTO dto);
        Task UpdateSubCategoryAsync(int id, CreateSubCategoryDTO dto);
        Task DeleteSubCategoryAsync(int id);
        Task<IEnumerable<SubCategoryDTO>> GetAllSubCategoriesAsync();
        Task<SubCategoryDTO> GetSubCategoryByIdAsync(int id);
    }
}