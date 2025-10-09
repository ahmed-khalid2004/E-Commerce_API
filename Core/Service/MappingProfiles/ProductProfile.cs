using AutoMapper;
using DomainLayer.Models.ProductModule;
using Services.MappingProfiles;
using Shared.DataTransferObjects.ProductModuleDTOs;

namespace ApplicationLayer.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product -> ProductDTO
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.ProductBrand.Name))
                .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.ProductType.Name))
                .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom<PictureUrlResolver>());
            // ProductType -> TypeDTO
            CreateMap<ProductType, TypeDTO>();

            // ProductBrand -> BrandDTO
            CreateMap<ProductBrand, BrandDTO>();
        }
    }
}