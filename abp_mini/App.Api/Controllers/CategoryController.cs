using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyApp.Api.Dtos;
using MyApp.Api.Services;
using MyLibrary.Application.Dtos;

namespace MyApp.Api.Controllers;

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
