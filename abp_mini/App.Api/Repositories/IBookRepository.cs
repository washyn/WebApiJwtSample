using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyApp.Api.Entities;
using MyApp.Api.Services;
using MyLibrary.Domain.Repositories;

namespace MyApp.Api.Repositories;

public interface IBookRepository : IRepository<Book, Guid>
{
    Task<List<Book>> GetListByAuthorAsync(string author);
    Task<long> GetCountAsync(BookFilter input);
    Task<List<Book>> GetPagedListAsync(BookFilter input);
}
