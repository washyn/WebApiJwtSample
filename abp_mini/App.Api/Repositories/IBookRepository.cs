using App.Api.Entities;
using App.Api.Services;

using Lib.Domain.Repositories;

namespace App.Api.Repositories;

public interface IBookRepository : IRepository<Book, Guid>
{
    Task<List<Book>> GetListByAuthorAsync(string author);
    Task<long> GetCountAsync(BookFilter input);
    Task<List<Book>> GetPagedListAsync(BookFilter input);
}