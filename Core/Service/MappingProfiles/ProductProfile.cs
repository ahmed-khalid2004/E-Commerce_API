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
                .ForMember(dest => dest.ProductType,
                           opt => opt.MapFrom(src => src.ProductType.Name))
                .ForMember(dest => dest.PictureUrl,
                           opt => opt.MapFrom<PictureUrlResolver>())
                .ForMember(dest => dest.Categories,
                           opt => opt.MapFrom(src =>
                               src.ProductCategories != null
                                   ? src.ProductCategories
                                         .Where(pc => pc.Category != null)
                                         .Select(pc => pc.Category.Name)
                                         .ToList()
                                   : new List<string>()));

            // ── CreateProductDTO → Product ────────────────────────────────────
            // CategoryIds handled manually in service (join table)
            CreateMap<CreateProductDTO, Product>()
                .ForMember(dest => dest.ProductCategories, opt => opt.Ignore());

            // ── UpdateProductDTO → Product ────────────────────────────────────
            CreateMap<UpdateProductDTO, Product>()
                .ForMember(dest => dest.ProductCategories, opt => opt.Ignore());

            // ── Brand ─────────────────────────────────────────────────────────
            CreateMap<ProductBrand, BrandDTO>();
            CreateMap<CreateBrandDTO, ProductBrand>();

            // ── Type ──────────────────────────────────────────────────────────
            CreateMap<ProductType, TypeDTO>();
            CreateMap<CreateTypeDTO, ProductType>();

            CreateMap<ProductReview, ProductReviewDTO>();

        }
    }
}