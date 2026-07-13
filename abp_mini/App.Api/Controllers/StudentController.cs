using App.Api.Dtos;
using App.Api.Services;

using Lib.Application.Dtos;

using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[Route("api/student")]
[ApiController]
public class StudentController : ControllerBase
{
    private readonly StudentAppService _studentAppService;

    public StudentController(StudentAppService studentAppService)
    {
        _studentAppService = studentAppService;
    }

    [HttpGet]
    public async Task<PagedResultDto<BookDto>> GetStudents([FromQuery] PagedAndSortedResultRequestDto input)
    {
        return await _studentAppService.GetListAsync(input);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetStudent(Guid id)
    {
        return await _studentAppService.GetAsync(id);
    }

    [HttpPost]
    public async Task<BookDto> CreateStudent([FromBody] BookDto studentDto)
    {
        return await _studentAppService.CreateAsync(studentDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BookDto>> UpdateStudent([FromRoute] Guid id, [FromBody] BookDto studentDto)
    {
        return await _studentAppService.UpdateAsync(id, studentDto);
    }

    [HttpDelete("{id}")]
    public async Task DeleteStudent(Guid id)
    {
        await _studentAppService.DeleteAsync(id);
    }
}