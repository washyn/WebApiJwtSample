using System.Threading.Tasks;
using MyLibrary.Application.Dtos;
using MyLibrary.Domain.Entities;
using MyLibrary.Domain.Repositories;

namespace MyLibrary.Application.Services;

public abstract class CrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TCreateInput, TUpdateInput> 
    : ReadOnlyAppService<TEntity, TEntityDto, TKey, TGetListInput>, 
      ICrudAppService<TEntityDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
    where TEntity : class, IEntity<TKey>
    where TEntityDto : class, IEntityDto<TKey>
    where TGetListInput : PagedAndSortedResultRequestDto
{
    protected new IRepository<TEntity, TKey> Repository => (IRepository<TEntity, TKey>)base.Repository;

    protected CrudAppService(IRepository<TEntity, TKey> repository) 
        : base(repository)
    {
    }

    public virtual async Task<TEntityDto> CreateAsync(TCreateInput input)
    {
        var entity = MapToEntity(input);

        await Repository.InsertAsync(entity, autoSave: true);

        return MapToEntityDto(entity);
    }

    public virtual async Task<TEntityDto> UpdateAsync(TKey id, TUpdateInput input)
    {
        var entity = await Repository.GetAsync(id);

        MapToEntity(input, entity);

        await Repository.UpdateAsync(entity, autoSave: true);

        return MapToEntityDto(entity);
    }

    public virtual async Task DeleteAsync(TKey id)
    {
        await Repository.DeleteAsync(id, autoSave: true);
    }

    protected virtual TEntity MapToEntity(TCreateInput createInput)
    {
        return ObjectMapper.Map<TCreateInput, TEntity>(createInput);
    }

    protected virtual void MapToEntity(TUpdateInput updateInput, TEntity entity)
    {
        ObjectMapper.Map(updateInput, entity);
    }
}

public abstract class CrudAppService<TEntity, TEntityDto, TKey>
    : CrudAppService<TEntity, TEntityDto, TKey, PagedAndSortedResultRequestDto, TEntityDto, TEntityDto>,
      ICrudAppService<TEntityDto, TKey>
    where TEntity : class, IEntity<TKey>
    where TEntityDto : class, IEntityDto<TKey>
{
    protected CrudAppService(IRepository<TEntity, TKey> repository)
        : base(repository)
    {
    }
}

public abstract class CrudAppService<TEntity, TEntityDto, TKey, TGetListInput>
    : CrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TEntityDto, TEntityDto>,
      ICrudAppService<TEntityDto, TKey, TGetListInput>
    where TEntity : class, IEntity<TKey>
    where TEntityDto : class, IEntityDto<TKey>
    where TGetListInput : PagedAndSortedResultRequestDto
{
    protected CrudAppService(IRepository<TEntity, TKey> repository)
        : base(repository)
    {
    }
}

public abstract class CrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TCreateInput>
    : CrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TCreateInput, TCreateInput>,
      ICrudAppService<TEntityDto, TKey, TGetListInput, TCreateInput>
    where TEntity : class, IEntity<TKey>
    where TEntityDto : class, IEntityDto<TKey>
    where TGetListInput : PagedAndSortedResultRequestDto
{
    protected CrudAppService(IRepository<TEntity, TKey> repository)
        : base(repository)
    {
    }
}

