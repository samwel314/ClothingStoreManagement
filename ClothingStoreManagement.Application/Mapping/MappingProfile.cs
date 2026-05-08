using AutoMapper;
using ClothingStoreManagement.Application.DTO;
using ClothingStoreManagement.Domain.Entities;

namespace ClothingStoreManagement.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Color, ColorListDTO>();
            CreateMap<Size, SizeListDTO>();
            CreateMap<Category, CategoryDTO>();
            CreateMap<Category, CategoryListDTO>();
        }
    }
}
