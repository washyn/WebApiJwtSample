using App.Api.Dtos;
using App.Api.Entities;

using AutoMapper;

namespace App.Api.ObjectMapping;

public class AppMappingProfile : Profile
{
    public AppMappingProfile()
    {
        // TodoItem mappings
        CreateMap<TodoItem, TodoItemDto>();
        CreateMap<CreateUpdateTodoItemDto, TodoItem>();

        // Book mappings
        CreateMap<Book, BookDto>().ReverseMap();
        CreateMap<CreateUpdateBookDto, Book>();

        // Category mappings
        CreateMap<Category, CategoryDto>();
    }
}
