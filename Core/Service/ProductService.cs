using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.ProductModule;
using Service.Specifications;
using Shared;
using Shared.DataTransferObjects.ProductModuleDTOs;

namespace Service
{
    public class ProductService(IUnitOfWork unitOfWork, IMapper mapper) : IProductService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        // ── Products Read ─────────────────────────────────────────────────────

        public async Task<PaginatedResult<ProductDTO>> GetAllProductsAsync(ProductQueryParams queryParams)
        {
            var repo = _unitOfWork.GetRepository<Product, int>();
            var listSpec = new ProductWithBrandAndTypeSpecifications(queryParams);
            var products = await repo.GetAllAsync(listSpec);
            var data = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductDTO>>(products);
            var countSpec = new ProductCountSpecifications(queryParams);
            var total = await repo.CountASync(countSpec);
            return new PaginatedResult<ProductDTO>(queryParams.PageNumber, products.Count(), total, data);
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            var spec = new ProductWithBrandAndTypeSpecifications(id);
            var product = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(spec);
            return _mapper.Map<Product, ProductDTO?>(product);
        }

        // ── Products Admin CRUD ───────────────────────────────────────────────

        public async Task<ProductDTO> CreateProductAsync(CreateProductDTO dto)
        {
            var product = _mapper.Map<CreateProductDTO, Product>(dto);

            // Resolve CategoryIds → ProductCategory join rows
            if (dto.CategoryIds.Count > 0)
            {
                var categoryRepo = _unitOfWork.GetRepository<Category, int>();
                foreach (var catId in dto.CategoryIds.Distinct())
                {
                    _ = await categoryRepo.GetByIdAsync(catId)
                        ?? throw new CategoryNotFoundException(catId);
                    product.ProductCategories.Add(new ProductCategory { CategoryId = catId });
                }
            }

            await _unitOfWork.GetRepository<Product, int>().AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            // Reload with full includes for the response
            var spec = new ProductWithBrandAndTypeSpecifications(product.Id);
            var created = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(spec);
            return _mapper.Map<Product, ProductDTO>(created!);
        }

        public async Task UpdateProductAsync(int id, UpdateProductDTO dto)
        {
            var repo = _unitOfWork.GetRepository<Product, int>();
            var spec = new ProductWithBrandAndTypeSpecifications(id);
            var product = await repo.GetByIdAsync(spec)
                ?? throw new ProductNotFoundException(id);

            _mapper.Map(dto, product);

            // Full replace of categories
            product.ProductCategories.Clear();
            if (dto.CategoryIds.Count > 0)
            {
                var categoryRepo = _unitOfWork.GetRepository<Category, int>();
                foreach (var catId in dto.CategoryIds.Distinct())
                {
                    _ = await categoryRepo.GetByIdAsync(catId)
                        ?? throw new CategoryNotFoundException(catId);
                    product.ProductCategories.Add(new ProductCategory { CategoryId = catId });
                }
            }

            repo.Update(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<Product, int>();
            var product = await repo.GetByIdAsync(id)
                ?? throw new ProductNotFoundException(id);
            repo.Remove(product);
            await _unitOfWork.SaveChangesAsync();
        }

        // ── Brands Read ───────────────────────────────────────────────────────

        public async Task<IEnumerable<BrandDTO>> GetAllBrandsAsync()
        {
            var brands = await _unitOfWork.GetRepository<ProductBrand, int>().GetAllAsync();
            return _mapper.Map<IEnumerable<ProductBrand>, IEnumerable<BrandDTO>>(brands);
        }

        public async Task<BrandDTO> GetBrandByIdAsync(int id)
        {
            var brand = await _unitOfWork.GetRepository<ProductBrand, int>().GetByIdAsync(id)
                ?? throw new BrandNotFoundException(id);
            return _mapper.Map<ProductBrand, BrandDTO>(brand);
        }

        // ── Brands Admin CRUD ─────────────────────────────────────────────────

        public async Task<BrandDTO> CreateBrandAsync(CreateBrandDTO dto)
        {
            var brand = _mapper.Map<CreateBrandDTO, ProductBrand>(dto);
            await _unitOfWork.GetRepository<ProductBrand, int>().AddAsync(brand);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ProductBrand, BrandDTO>(brand);
        }

        public async Task UpdateBrandAsync(int id, CreateBrandDTO dto)
        {
            var repo = _unitOfWork.GetRepository<ProductBrand, int>();
            var brand = await repo.GetByIdAsync(id)
                ?? throw new BrandNotFoundException(id);
            _mapper.Map(dto, brand);
            repo.Update(brand);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteBrandAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<ProductBrand, int>();
            var brand = await repo.GetByIdAsync(id)
                ?? throw new BrandNotFoundException(id);
            repo.Remove(brand);
            await _unitOfWork.SaveChangesAsync();
        }

        // ── Types Read ────────────────────────────────────────────────────────

        public async Task<IEnumerable<TypeDTO>> GetAllTypesAsync()
        {
            var types = await _unitOfWork.GetRepository<ProductType, int>().GetAllAsync();
            return _mapper.Map<IEnumerable<ProductType>, IEnumerable<TypeDTO>>(types);
        }

        public async Task<TypeDTO> GetTypeByIdAsync(int id)
        {
            var type = await _unitOfWork.GetRepository<ProductType, int>().GetByIdAsync(id)
                ?? throw new ProductTypeNotFoundException(id);
            return _mapper.Map<ProductType, TypeDTO>(type);
        }

        // ── Types Admin CRUD ──────────────────────────────────────────────────

        public async Task<TypeDTO> CreateTypeAsync(CreateTypeDTO dto)
        {
            var type = _mapper.Map<CreateTypeDTO, ProductType>(dto);
            await _unitOfWork.GetRepository<ProductType, int>().AddAsync(type);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ProductType, TypeDTO>(type);
        }

        public async Task UpdateTypeAsync(int id, CreateTypeDTO dto)
        {
            var repo = _unitOfWork.GetRepository<ProductType, int>();
            var type = await repo.GetByIdAsync(id)
                ?? throw new ProductTypeNotFoundException(id);
            _mapper.Map(dto, type);
            repo.Update(type);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteTypeAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<ProductType, int>();
            var type = await repo.GetByIdAsync(id)
                ?? throw new ProductTypeNotFoundException(id);
            repo.Remove(type);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}