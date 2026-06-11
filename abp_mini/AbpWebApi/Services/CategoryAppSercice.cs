using App.Api.Entities;

using Library.Application.Dtos;
using Library.Application.Services;
using Library.Domain.Entities;
using Library.Domain.Repositories;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.DependencyInjection;

namespace App.Api.Services;

public class CategoryDto : EntityDto<Guid>
{
    public string Name { get; set; } = string.Empty;
}

public class CategoryAppService : CrudAppService<Category, CategoryDto, Guid>, ITransientDependency
{
    public CategoryAppService(IRepository<Category, Guid> repository) : base(repository)
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
    public async Task<PagedResultDto<CategoryDto>> GetCategories()
    {
        return await _categoryAppService.GetListAsync(new PagedAndSortedResultRequestDto());
    }

    [HttpGet("{id}")]
    public async Task<CategoryDto> GetCategory(Guid id)
    {
        return await _categoryAppService.GetAsync(id);
    }

    [HttpPost]
    public async Task<CategoryDto> CreateCategory([FromBody] CategoryDto categoryDto)
    {
        return await _categoryAppService.CreateAsync(categoryDto);
    }

    [HttpPut("{id}")]
    public async Task<CategoryDto> UpdateCategory([FromRoute] Guid id, [FromBody] CategoryDto categoryDto)
    {
        return await _categoryAppService.UpdateAsync(id, categoryDto);
    }

    [HttpDelete("{id}")]
    public async Task DeleteCategory(Guid id)
    {
        await _categoryAppService.DeleteAsync(id);
    }
}