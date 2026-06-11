using System;
using MyApp.Api.Dtos;
using MyApp.Api.Entities;
using MyLibrary.Application.Dtos;
using MyLibrary.Application.Services;
using MyLibrary.Domain.Repositories;

namespace MyApp.Api.Services;

public class TodoAppService : CrudAppService<TodoItem, TodoItemDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateTodoItemDto, CreateUpdateTodoItemDto>
{
    public TodoAppService(IRepository<TodoItem, Guid> repository) 
        : base(repository)
    {
    }
}
