using System;
using MyApp.Api.Dtos;
using MyApp.Api.Entities;
using MyLibrary.Application.Dtos;
using MyLibrary.Application.Services;
using MyLibrary.Domain.Repositories;

namespace MyApp.Api.Services;

public class StudentAppService : CrudAppService<Book, BookDto, Guid>
{
    public StudentAppService(IRepository<Book, Guid> repository) 
        : base(repository)
    {
    }
}
