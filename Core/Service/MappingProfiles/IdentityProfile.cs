using AutoMapper;
using DomainLayer.Models.IdentityModule;
using Shared.DataTransferObjects.IdentityDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.MappingProfiles
{
    public class IdentityProfile : Profile
    {
        public IdentityProfile() { 
        CreateMap<Address,AddressDTO>().ReverseMap();
        }
    }
}
