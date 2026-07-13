using App.Api.Dtos;
using App.Api.Entities;

using Library.Application.Services;
using Library.Domain.Repositories;

using Volo.Abp.DependencyInjection;

namespace App.Api.Services;

public class StudentAppService : CrudAppService<Book, BookDto, Guid>, ITransientDependency
{
    public StudentAppService(IRepository<Book, Guid> repository)
        : base(repository)
    {
    }
}