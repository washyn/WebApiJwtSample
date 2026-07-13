using App.Api.Dtos;
using App.Api.Services;

using Lib.Application.Dtos;

using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[ApiController]
[Route("api/books")]
public class BookController : ControllerBase
{
    private readonly BookAppService _bookAppService;

    public BookController(BookAppService bookAppService)
    {
        _bookAppService = bookAppService;
    }

    [HttpGet]
    public async Task<PagedResultDto<BookDto>> GetListAsync([FromQuery] BookFilter input)
    {
        return await _bookAppService.GetListAsync(input);
    }

    [HttpGet("{id}")]
    public async Task<BookDto> GetAsync(Guid id)
    {
        return await _bookAppService.GetAsync(id);
    }

    [HttpGet("by-author")]
    public async Task<System.Collections.Generic.List<BookDto>> GetByAuthorAsync([FromQuery] string author)
    {
        return await _bookAppService.GetByAuthorAsync(author);
    }

    [HttpPost]
    public async Task<BookDto> CreateAsync(CreateUpdateBookDto input)
    {
        return await _bookAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public async Task<BookDto> UpdateAsync(Guid id, CreateUpdateBookDto input)
    {
        return await _bookAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await _bookAppService.DeleteAsync(id);
    }
}