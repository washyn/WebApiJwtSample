using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyApp.Api.Dtos;
using MyApp.Api.Services;
using MyLibrary.Application.Dtos;

namespace MyApp.Api.Controllers;

[ApiController]
[Route("api/todos")]
public class TodoController : ControllerBase
{
    private readonly TodoAppService _todoAppService;

    public TodoController(TodoAppService todoAppService)
    {
        _todoAppService = todoAppService;
    }

    [HttpGet]
    public async Task<PagedResultDto<TodoItemDto>> GetListAsync([FromQuery] PagedAndSortedResultRequestDto input)
    {
        return await _todoAppService.GetListAsync(input);
    }

    [HttpGet("{id}")]
    public async Task<TodoItemDto> GetAsync(Guid id)
    {
        return await _todoAppService.GetAsync(id);
    }

    [HttpPost]
    public async Task<TodoItemDto> CreateAsync(CreateUpdateTodoItemDto input)
    {
        return await _todoAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public async Task<TodoItemDto> UpdateAsync(Guid id, CreateUpdateTodoItemDto input)
    {
        return await _todoAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await _todoAppService.DeleteAsync(id);
    }
}
