using App.Api.Dtos;
using App.Api.Services;

using Library.Application.Dtos;

using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly CategoryAppService _categoryAppService;

    public CategoryController(CategoryAppService categoryAppService)
    {
        _categoryAppService = categoryAppService;
    }

    [HttpGet]
    public async Task<PagedResultDto<CategoryDto>> GetListAsync([FromQuery] PagedAndSortedResultRequestDto input)
    {
        return await _categoryAppService.GetListAsync(input);
    }

    [HttpGet("{id}")]
    public async Task<CategoryDto> GetAsync(Guid id)
    {
        return await _categoryAppService.GetAsync(id);
    }
}
