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
        }
    }
}
