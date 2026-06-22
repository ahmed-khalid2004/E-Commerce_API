using AutoMapper;
using DomainLayer.Models.IdentityModule;
using Shared.DataTransferObjects.IdentityDTOs;

namespace Service.MappingProfiles
{
    public class IdentityProfile : Profile
    {
        public IdentityProfile()
        {
            CreateMap<Address, AddressDTO>().ReverseMap();
            CreateMap<ApplicationUser, CustomerDTO>();
        }
    }
}
