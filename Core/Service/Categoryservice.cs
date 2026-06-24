using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.ProductModule;
using Service.Specifications;
using ServiceAbstracion;
using Shared.DataTransferObjects.ProductModuleDTOs;

namespace Service
{
    public class CategoryService(IUnitOfWork unitOfWork, IMapper mapper) : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        // ── Category ──────────────────────────────────────────────────────────

        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            var spec = new CategoryWithSubCategoriesSpecifications();
            var categories = await _unitOfWork.GetRepository<Category, int>().GetAllAsync(spec);
            return _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(int id)
        {
            var spec = new CategoryWithSubCategoriesSpecifications(id);
            var category = await _unitOfWork.GetRepository<Category, int>().GetByIdAsync(spec)
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

            _mapper.Map(dto, category);
            repo.Update(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<Category, int>();
            var category = await repo.GetByIdAsync(id)
                ?? throw new CategoryNotFoundException(id);

            // DeleteBehavior.Restrict on SubCategory -> Category will throw a DB
            // exception here if SubCategories still exist — that's intentional:
            // forces an explicit decision instead of silently cascading.
            repo.Remove(category);
            await _unitOfWork.SaveChangesAsync();
        }

        // ── SubCategory ───────────────────────────────────────────────────────

        public async Task<IEnumerable<SubCategoryDTO>> GetAllSubCategoriesAsync()
        {
            var subCategories = await _unitOfWork.GetRepository<SubCategory, int>().GetAllAsync();
            return _mapper.Map<IEnumerable<SubCategory>, IEnumerable<SubCategoryDTO>>(subCategories);
        }

        public async Task<SubCategoryDTO> GetSubCategoryByIdAsync(int id)
        {
            var subCategory = await _unitOfWork.GetRepository<SubCategory, int>().GetByIdAsync(id)
                ?? throw new SubCategoryNotFoundException(id);

            return _mapper.Map<SubCategory, SubCategoryDTO>(subCategory);
        }

        public async Task<SubCategoryDTO> CreateSubCategoryAsync(CreateSubCategoryDTO dto)
        {
            // Verify the parent Category exists before creating the child
            var categoryRepo = _unitOfWork.GetRepository<Category, int>();
            _ = await categoryRepo.GetByIdAsync(dto.CategoryId)
                ?? throw new CategoryNotFoundException(dto.CategoryId);

            var subCategory = _mapper.Map<CreateSubCategoryDTO, SubCategory>(dto);
            await _unitOfWork.GetRepository<SubCategory, int>().AddAsync(subCategory);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SubCategory, SubCategoryDTO>(subCategory);
        }

        public async Task UpdateSubCategoryAsync(int id, CreateSubCategoryDTO dto)
        {
            var repo = _unitOfWork.GetRepository<SubCategory, int>();
            var subCategory = await repo.GetByIdAsync(id)
                ?? throw new SubCategoryNotFoundException(id);

            if (dto.CategoryId != subCategory.CategoryId)
            {
                var categoryRepo = _unitOfWork.GetRepository<Category, int>();
                _ = await categoryRepo.GetByIdAsync(dto.CategoryId)
                    ?? throw new CategoryNotFoundException(dto.CategoryId);
            }

            _mapper.Map(dto, subCategory);
            repo.Update(subCategory);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteSubCategoryAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<SubCategory, int>();
            var subCategory = await repo.GetByIdAsync(id)
                ?? throw new SubCategoryNotFoundException(id);

            repo.Remove(subCategory);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}