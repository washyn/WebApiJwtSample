namespace WebAppPagination.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Washyn.UNAJ.Sorteo.HttpApi.Entities;
using Washyn.UNAJ.Sorteo.HttpApi.Services;

public class AppController
{
}

[AllowAnonymous]
public class ExampleAppService : ApplicationService, IReadOnlyAppService<DocenteDto, Guid>
{
    private readonly IRepository<Docente, Guid> _repository;

    public ExampleAppService(IRepository<Docente, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<DocenteDto> GetAsync(Guid id)
    {
        var docente = await _repository.GetAsync(id);
        return ObjectMapper.Map<Docente, DocenteDto>(docente);
    }

    [Obsolete]
    public Task<PagedResultDto<DocenteDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        throw new NotImplementedException();
    }

    // TODO: implementar filtro de paginación con el repositorio
    public async Task<PagedResponseDto<DocenteDto>> GetPagedAsync(ExampleFilterDto input)
    {
        var skip = (input.PageNumber - 1) * input.PageSize;
        var queryable = await _repository.GetQueryableAsync();
        queryable = queryable.Skip(skip).Take(input.PageSize);
        return new PagedResponseDto<DocenteDto>
        {
            Items = ObjectMapper.Map<List<Docente>, List<DocenteDto>>(await queryable.ToListAsync()),
            PageSize = input.PageSize,
            PageNumber = input.PageNumber,
            TotalItems = await _repository.GetCountAsync(),
        };
    }
}

public class ExampleFilterDto : PagedRequestDto
{
    public string? Filter { get; set; }
}

public class PagedRequestDto
{
    /// <summary>
    /// Página solicitada (por defecto 1).
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Cantidad de elementos por página (por defecto 10).
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Campo para ordenar (opcional).
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Dirección del orden: "asc" o "desc" (opcional).
    /// </summary>
    public string? SortDirection { get; set; } = "asc";
}

public class PagedResponseDto<T>
{
    /// <summary>
    /// Elementos de la página actual.
    /// </summary>
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    /// <summary>
    /// Página actual.
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Tamaño de la página.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de elementos.
    /// </summary>
    public long TotalItems { get; set; }


    // /// <summary>
    // /// Total de páginas.
    // /// </summary>
    // public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    //
    // //Opcional links para paginación HATEOAS
    //
    // /// <summary>
    // /// URL de la página anterior.
    // /// </summary>
    // public string? PreviousPage { get; set; }
    //
    // /// <summary>
    // /// URL de la página siguiente.
    // /// </summary>
    // public string? NextPage { get; set; }
}