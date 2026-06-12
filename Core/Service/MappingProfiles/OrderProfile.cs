using AutoMapper;
using DomainLayer.Models.OrderModule;
using Shared.DataTransferObjects.IdentityDTOs;
using Shared.DataTransferObjects.OrderDTOs;

namespace Service.MappingProfiles
{
    class OrderProfile : Profile
    {
        public OrderProfile()
        {
            // DTO → OrderAddress (used when creating an order)
            CreateMap<AddressDTO, OrderAddress>();

            // OrderAddress → DTO (used when returning an order) — was missing
            CreateMap<OrderAddress, AddressDTO>();

            CreateMap<Order, OrderToReturnDTO>()
                .ForMember(dest => dest.DeliveryMethod,
                           opt => opt.MapFrom(src => src.DeliveryMethod.ShortName))
                .ForMember(dest => dest.DeliveryCost,
                           opt => opt.MapFrom(src => src.DeliveryMethod.Cost))
                .ForMember(dest => dest.Status,
                           opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Total,
                           opt => opt.MapFrom(src => src.Subtotal + src.DeliveryMethod.Cost));

            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.ProductName,
                           opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.PictureUrl,
                           opt => opt.MapFrom<OrderItemPictureUrlResolver>());

            CreateMap<DeliveryMethod, DeliveryMethodDTO>();
        }
    }
}