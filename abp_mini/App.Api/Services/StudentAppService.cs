using App.Api.Dtos;
using App.Api.Entities;

using Library.Application.Services;
using Library.Domain.Repositories;

namespace App.Api.Services;

public class StudentAppService : CrudAppService<Book, BookDto, Guid>
{
    public StudentAppService(IRepository<Book, Guid> repository) 
        : base(repository)
    {
    }
}
