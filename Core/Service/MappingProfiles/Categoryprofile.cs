using AutoMapper;
using DomainLayer.Models.ProductModule;
using Shared.DataTransferObjects.ProductModuleDTOs;

namespace Service.MappingProfiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            // Category → CategoryDTO
            CreateMap<Category, CategoryDTO>();

            // CreateCategoryDTO → Category
            CreateMap<CreateCategoryDTO, Category>();
        }
    }
}