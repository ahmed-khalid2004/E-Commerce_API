using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.ProductModule;
using ServiceAbstracion;
using Shared.DataTransferObjects.ProductModuleDTOs;

namespace Service
{
    public class CategoryService(IUnitOfWork unitOfWork, IMapper mapper) : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.GetRepository<Category, int>().GetAllAsync();
            return _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork.GetRepository<Category, int>().GetByIdAsync(id)
                ?? throw new CategoryNotFoundException(id);

            return _mapper.Map<Category, CategoryDTO>(category);
        }

        public async Task<CategoryDTO> CreateCategoryAsync(CreateCategoryDTO dto)
        {
            var category = _mapper.Map<CreateCategoryDTO, Category>(dto);
            await _unitOfWork.GetRepository<Category, int>().AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<Category, CategoryDTO>(category);
        }

        public async Task UpdateCategoryAsync(int id, CreateCategoryDTO dto)
        {
            var repo = _unitOfWork.GetRepository<Category, int>();
            var category = await repo.GetByIdAsync(id)
                ?? throw new CategoryNotFoundException(id);

            // Map updated values onto the tracked entity
            _mapper.Map(dto, category);
            repo.Update(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<Category, int>();
            var category = await repo.GetByIdAsync(id)
                ?? throw new CategoryNotFoundException(id);

            repo.Remove(category);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}