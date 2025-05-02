using AutoMapper;
using DomainLayer.Models;
using Shared.DataTransferObjects;

namespace ApplicationLayer.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product -> ProductDTO
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.BrandName,opt => opt.MapFrom(src => src.ProductBrand.Name))
                .ForMember(dest => dest.TypeName,opt => opt.MapFrom(src => src.ProductType.Name));

            // ProductType -> TypeDTO
            CreateMap<ProductType, TypeDTO>();

            // ProductBrand -> BrandDTO
            CreateMap<ProductBrand, BrandDTO>();
        }
    }
}