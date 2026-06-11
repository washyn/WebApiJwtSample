using System.Linq.Dynamic.Core;

using App.Api.Data;
using App.Api.Entities;
using App.Api.Services;

using Library.Domain.Repositories;
using Library.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;

namespace App.Api.Repositories;

public class BookRepository : EfCoreRepository<AppDbContext, Book, Guid>, IBookRepository
{
    public BookRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<Book>> GetListByAuthorAsync(string author)
    {
        var query = await GetQueryableAsync();
        return await query.Where(b => b.Author.Contains(author)).ToListAsync();
    }

    public async Task<long> GetCountAsync(BookFilter input)
    {
        var queryable = GetQueryable();
        var query = ApplyFilter(queryable, input);
        return await query.LongCountAsync();
    }

    public async Task<List<Book>> GetPagedListAsync(BookFilter input)
    {
        var query = ApplyFilter(GetQueryable(), input);
        query = query.OrderBy(string.IsNullOrWhiteSpace(input.Sorting) ? "id asc" : input.Sorting);
        return await query.PageBy(input.SkipCount, input.MaxResultCount).ToListAsync();
    }

    protected virtual IQueryable<Book> ApplyFilter(
        IQueryable<Book> query,
        BookFilter input)
    {
        return query.WhereIf(!string.IsNullOrEmpty(input.Filter),
            b => b.Title.Contains(input.Filter) || b.Author.Contains(input.Filter));
    }
}
