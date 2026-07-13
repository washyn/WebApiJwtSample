using App.Api.Dtos;
using App.Api.Entities;
using App.Api.Repositories;

using Library.Application.Dtos;
using Library.Application.Services;

using Volo.Abp.DependencyInjection;

namespace App.Api.Services;

public class BookAppService : CrudAppService<Book, BookDto, Guid, BookFilter, CreateUpdateBookDto,
    CreateUpdateBookDto>, ITransientDependency
{
    private readonly IBookRepository _bookRepository;

    public BookAppService(IBookRepository repository)
        : base(repository)
    {
        _bookRepository = repository;
    }

    // Custom method utilizing the custom repository
    public async Task<List<BookDto>> GetByAuthorAsync(string author)
    {
        var books = await _bookRepository.GetListByAuthorAsync(author);

        var dtoList = new List<BookDto>();
        foreach (var book in books)
        {
            dtoList.Add(ObjectMapper.Map<Book, BookDto>(book));
        }

        return dtoList;
    }

    // Overriding a base method to add custom business logic
    public override async Task<BookDto> CreateAsync(CreateUpdateBookDto input)
    {
        // Custom logic: Set default author if not provided
        if (string.IsNullOrWhiteSpace(input.Author))
        {
            input.Author = "Unknown Author";
        }

        return await base.CreateAsync(input);
    }

    public override Task<BookDto> GetAsync(Guid id)
    {
        return base.GetAsync(id);
    }

    public override async Task<PagedResultDto<BookDto>> GetListAsync(BookFilter input)
    {
        // skip filter
        var count = await _bookRepository.GetCountAsync(input);
        var books = await _bookRepository.GetPagedListAsync(input);
        var mapped = books.Select(b => ObjectMapper.Map<Book, BookDto>(b)).ToList();
        return new PagedResultDto<BookDto>(count, mapped);
    }

    public override Task<BookDto> UpdateAsync(Guid id, CreateUpdateBookDto input)
    {
        return base.UpdateAsync(id, input);
    }

    public override Task DeleteAsync(Guid id)
    {
        return base.DeleteAsync(id);
    }

    protected override IQueryable<Book> ApplyDefaultSorting(IQueryable<Book> query)
    {
        return base.ApplyDefaultSorting(query);
    }

    protected override IQueryable<Book> ApplyPaging(IQueryable<Book> query, BookFilter input)
    {
        return base.ApplyPaging(query, input);
    }

    protected override IQueryable<Book> ApplySorting(IQueryable<Book> query, BookFilter input)
    {
        return base.ApplySorting(query, input);
    }

    protected override IQueryable<Book> CreateFilteredQuery(BookFilter input)
    {
        return base.CreateFilteredQuery(input);
    }


    protected override BookDto MapToGetOutputDto(Book entity)
    {
        return base.MapToGetOutputDto(entity);
    }

    protected override BookDto MapToGetListOutputDto(Book entity)
    {
        return base.MapToGetListOutputDto(entity);
    }
}

public class BookFilter : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}