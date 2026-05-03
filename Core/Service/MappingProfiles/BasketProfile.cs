using AutoMapper;
using DomainLayer.Models.BasketModule;
using Shared.DataTransferObjects.BasketModuleDTOs;


namespace Service.MappingProfiles
{
    class BasketProfile : Profile
    {
        public BasketProfile()
        {
            CreateMap<CustomerBasket, BasketDTO>().ReverseMap();
            CreateMap<BasketItem, BasketItemDTO>().ReverseMap();
        }
    }
}
