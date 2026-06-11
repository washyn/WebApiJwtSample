using App.Api.Dtos;
using App.Api.Entities;

using Library.Application.Dtos;
using Library.Application.Services;
using Library.Domain.Entities;
using Library.Domain.Repositories;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.DependencyInjection;

namespace App.Api.Services;

public class CategoryAppService : CrudAppService<Book, BookDto, Guid>, ITransientDependency
{
    public CategoryAppService(IRepository<Book, Guid> repository) : base(repository)
    {
    }
}

[Route("api/category")]
[ApiController]
public class CategoryController : AbpController
{
    private readonly CategoryAppService _categoryAppService;

    public CategoryController(CategoryAppService categoryAppService)
    {
        _categoryAppService = categoryAppService;
    }

    [HttpGet]
    public async Task<PagedResultDto<BookDto>> GetCategories()
    {
        return await _categoryAppService.GetListAsync(new PagedAndSortedResultRequestDto());
    }

    [HttpGet("{id}")]
    public async Task<BookDto> GetCategory(Guid id)
    {
        return await _categoryAppService.GetAsync(id);
    }

    [HttpPost]
    public async Task<BookDto> CreateCategory([FromBody] BookDto categoryDto)
    {
        return await _categoryAppService.CreateAsync(categoryDto);
    }

    [HttpPut("{id}")]
    public async Task<BookDto> UpdateCategory([FromRoute] Guid id, [FromBody] BookDto categoryDto)
    {
        return await _categoryAppService.UpdateAsync(id, categoryDto);
    }

    [HttpDelete("{id}")]
    public async Task DeleteCategory(Guid id)
    {
        await _categoryAppService.DeleteAsync(id);
    }
}