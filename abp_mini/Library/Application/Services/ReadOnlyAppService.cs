using Lib.Application.Dtos;
using Lib.Domain.Entities;
using Lib.Domain.Repositories;

namespace Lib.Application.Services;

public abstract class ReadOnlyAppService<TEntity, TEntityDto, TKey, TGetListInput>
    : ApplicationService, IReadOnlyAppService<TEntityDto, TKey, TGetListInput>
    where TEntity : class, IEntity<TKey>
    where TEntityDto : class, IEntityDto<TKey>
    where TGetListInput : PagedAndSortedResultRequestDto
{
    protected IReadOnlyRepository<TEntity, TKey> Repository { get; }

    protected ReadOnlyAppService(IReadOnlyRepository<TEntity, TKey> repository)
    {
        Repository = repository;
    }

    public virtual async Task<TEntityDto> GetAsync(TKey id)
    {
        var entity = await Repository.GetAsync(id);
        return MapToGetOutputDto(entity);
    }

    public virtual async Task<PagedResultDto<TEntityDto>> GetListAsync(TGetListInput input)
    {
        var query = CreateFilteredQuery(input);
        var totalCount = query.Count();

        query = ApplySorting(query, input);
        query = ApplyPaging(query, input);

        var entities = query.ToList();

        return new PagedResultDto<TEntityDto>(
            totalCount,
            entities.Select(MapToGetListOutputDto).ToList()
        );
    }

    protected virtual System.Linq.IQueryable<TEntity> CreateFilteredQuery(TGetListInput input)
    {
        return Repository.GetQueryable();
    }

    protected virtual System.Linq.IQueryable<TEntity> ApplySorting(System.Linq.IQueryable<TEntity> query,
        TGetListInput input)
    {
        if (!string.IsNullOrWhiteSpace(input.Sorting))
        {
            return System.Linq.Dynamic.Core.DynamicQueryableExtensions.OrderBy(query, input.Sorting);
        }

        return ApplyDefaultSorting(query);
    }

    protected virtual System.Linq.IQueryable<TEntity> ApplyDefaultSorting(System.Linq.IQueryable<TEntity> query)
    {
        return System.Linq.Dynamic.Core.DynamicQueryableExtensions.OrderBy(query, "Id asc");
    }

    protected virtual System.Linq.IQueryable<TEntity> ApplyPaging(System.Linq.IQueryable<TEntity> query,
        TGetListInput input)
    {
        return System.Linq.Queryable.Take(System.Linq.Queryable.Skip(query, input.SkipCount), input.MaxResultCount);
    }

    protected virtual TEntityDto MapToEntityDto(TEntity entity)
    {
        return ObjectMapper.Map<TEntity, TEntityDto>(entity);
    }

    protected virtual TEntityDto MapToGetOutputDto(TEntity entity)
    {
        return MapToEntityDto(entity);
    }

    protected virtual TEntityDto MapToGetListOutputDto(TEntity entity)
    {
        return MapToEntityDto(entity);
    }
}

public abstract class ReadOnlyAppService<TEntity, TEntityDto, TKey>
    : ReadOnlyAppService<TEntity, TEntityDto, TKey, PagedAndSortedResultRequestDto>,
      IReadOnlyAppService<TEntityDto, TKey>
    where TEntity : class, IEntity<TKey>
    where TEntityDto : class, IEntityDto<TKey>
{
    protected ReadOnlyAppService(IReadOnlyRepository<TEntity, TKey> repository)
        : base(repository)
    {
    }
}

