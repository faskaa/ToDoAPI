using AutoMapper;
using ToDoApp.DTOs;
using ToDoApp.Entities;

namespace ToDoApp.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Todo, TodoGetDTO>();
            CreateMap<Todo, TodoCreateDTO>();
        }
    }
}
