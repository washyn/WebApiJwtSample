using System;
using MyApp.Api.Dtos;
using MyApp.Api.Entities;
using MyLibrary.Application.Dtos;
using MyLibrary.Application.Services;
using MyLibrary.Domain.Repositories;

namespace MyApp.Api.Services;

public class CategoryAppService : ReadOnlyAppService<Category, CategoryDto, Guid, PagedAndSortedResultRequestDto>
{
    public CategoryAppService(IReadOnlyRepository<Category, Guid> repository) 
        : base(repository)
    {
    }
}
