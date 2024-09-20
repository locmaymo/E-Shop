using AutoMapper;
using Catalog.API.DTOs;
using Catalog.API.Models;

namespace Catalog.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //category
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Category, CategoryPostDTO>().ReverseMap();
            CreateMap<Category, CategoryPutDTO>().ReverseMap();

            //product
            CreateMap<Product, ProductGetDTO>().ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name ?? string.Empty));
            CreateMap<Product, ProductPostDTO>().ReverseMap();
            CreateMap<Product, ProductPutDTO>().ReverseMap();
        }
    }
}
