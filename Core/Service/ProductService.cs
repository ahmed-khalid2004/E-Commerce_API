using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.ProductModule;
using Microsoft.EntityFrameworkCore;
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
            await _unitOfWork.GetRepository<Product, int>().AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            var spec = new ProductWithBrandAndTypeSpecifications(product.Id);
            var created = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(spec);
            return _mapper.Map<Product, ProductDTO>(created!);
        }

        public async Task UpdateProductAsync(int id, UpdateProductDTO dto)
        {
            var repo = _unitOfWork.GetRepository<Product, int>();
            var product = await repo.GetByIdAsync(id)
                ?? throw new ProductNotFoundException(id);

            _mapper.Map(dto, product);
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

        // ── Derived filter data — no Brand<->SubCategory relation in schema ────
        // These queries infer the relationship live from Products, exactly as
        // discussed: Product is the single source of truth for which Brands
        // appear under which SubCategory/Category.

        public async Task<IEnumerable<BrandDTO>> GetBrandsBySubCategoryAsync(int subCategoryId)
        {
            // Verify the SubCategory exists — fail fast with a clear 404
            var subCategoryRepo = _unitOfWork.GetRepository<SubCategory, int>();
            _ = await subCategoryRepo.GetByIdAsync(subCategoryId)
                ?? throw new SubCategoryNotFoundException(subCategoryId);

            var spec = new DistinctBrandsBySubCategorySpecifications(subCategoryId);
            var brands = await _unitOfWork.GetRepository<ProductBrand, int>().GetAllAsync(spec);
            return _mapper.Map<IEnumerable<ProductBrand>, IEnumerable<BrandDTO>>(brands);
        }

        public async Task<IEnumerable<BrandDTO>> GetBrandsByCategoryAsync(int categoryId)
        {
            var categoryRepo = _unitOfWork.GetRepository<Category, int>();
            _ = await categoryRepo.GetByIdAsync(categoryId)
                ?? throw new CategoryNotFoundException(categoryId);

            var spec = new DistinctBrandsByCategorySpecifications(categoryId);
            var brands = await _unitOfWork.GetRepository<ProductBrand, int>().GetAllAsync(spec);
            return _mapper.Map<IEnumerable<ProductBrand>, IEnumerable<BrandDTO>>(brands);
        }
    }
}