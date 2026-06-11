using App.Api.Dtos;
using App.Api.Entities;

using Library.Application.Dtos;
using Library.Application.Services;
using Library.Domain.Repositories;

namespace App.Api.Services;

public class CategoryAppService : ReadOnlyAppService<Category, CategoryDto, Guid, PagedAndSortedResultRequestDto>
{
    public CategoryAppService(IReadOnlyRepository<Category, Guid> repository) 
        : base(repository)
    {
    }
}
