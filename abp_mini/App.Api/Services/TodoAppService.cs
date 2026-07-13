using App.Api.Dtos;
using App.Api.Entities;

using Library.Application.Dtos;
using Library.Application.Services;
using Library.Domain.Repositories;

using Volo.Abp.DependencyInjection;

namespace App.Api.Services;

public class TodoAppService : CrudAppService<TodoItem, TodoItemDto, Guid, PagedAndSortedResultRequestDto,
    CreateUpdateTodoItemDto, CreateUpdateTodoItemDto>, ITransientDependency
{
    public TodoAppService(IRepository<TodoItem, Guid> repository)
        : base(repository)
    {
    }
}