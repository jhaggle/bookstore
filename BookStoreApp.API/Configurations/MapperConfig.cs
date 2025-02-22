using AutoMapper;
using BookStoreApp.API.Data;
using BookStoreApp.API.Models.Author;

namespace BookStoreApp.API.Configurations
{
    public class MapperConfig : Profile // Den extendar grejer från automapper
    {

        public MapperConfig()
        {
            CreateMap<AuthorCreateDto, Author>().ReverseMap();
            CreateMap<AuthorReadOnlyDto, Author>().ReverseMap(); //read operation
            CreateMap<AuthorUpdateDto, Author>().ReverseMap();

        }

    }
}
