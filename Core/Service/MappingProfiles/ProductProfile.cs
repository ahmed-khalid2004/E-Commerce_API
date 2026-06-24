using AutoMapper;
using DomainLayer.Models.ProductModule;
using Service.MappingProfiles;
using Services.MappingProfiles;
using Shared.DataTransferObjects.ProductModuleDTOs;

namespace Service.MappingProfiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            // ── Product → ProductDTO ──────────────────────────────────────────
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.ProductBrand,
                           opt => opt.MapFrom(src => src.ProductBrand.Name))
                .ForMember(dest => dest.PictureUrl,
                           opt => opt.MapFrom<PictureUrlResolver>())
                .ForMember(dest => dest.SubCategory,
                           opt => opt.MapFrom(src => src.SubCategory.Name))
                .ForMember(dest => dest.CategoryId,
                           opt => opt.MapFrom(src => src.SubCategory.CategoryId))
                .ForMember(dest => dest.CategoryName,
                           opt => opt.MapFrom(src => src.SubCategory.Category.Name));

            // ── CreateProductDTO / UpdateProductDTO → Product ─────────────────
            CreateMap<CreateProductDTO, Product>();
            CreateMap<UpdateProductDTO, Product>();

            // ── Brand ─────────────────────────────────────────────────────────
            CreateMap<ProductBrand, BrandDTO>();
            CreateMap<CreateBrandDTO, ProductBrand>();

            CreateMap<SubCategory, SubCategoryDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            // ── Category — includes nested SubCategories ──────────────────────
            CreateMap<Category, CategoryDTO>();
            CreateMap<CreateCategoryDTO, Category>();

            // ── Reviews (unchanged) ────────────────────────────────────────────
            CreateMap<ProductReview, ProductReviewDTO>();
        }
    }
}