using Lib.Application.Dtos;

namespace Lib.Application.Services;

public interface ICrudAppService<TEntityDto, in TKey, in TGetListInput, in TCreateInput, in TUpdateInput> 
    : IReadOnlyAppService<TEntityDto, TKey, TGetListInput>
    where TEntityDto : class, IEntityDto<TKey>
{
    Task<TEntityDto> CreateAsync(TCreateInput input);

    Task<TEntityDto> UpdateAsync(TKey id, TUpdateInput input);

    Task DeleteAsync(TKey id);
}

public interface ICrudAppService<TEntityDto, TKey>
    : ICrudAppService<TEntityDto, TKey, PagedAndSortedResultRequestDto>
    where TEntityDto : class, IEntityDto<TKey>
{
}

public interface ICrudAppService<TEntityDto, TKey, in TGetListInput>
    : ICrudAppService<TEntityDto, TKey, TGetListInput, TEntityDto>
    where TEntityDto : class, IEntityDto<TKey>
{
}

public interface ICrudAppService<TEntityDto, TKey, in TGetListInput, in TCreateInput>
    : ICrudAppService<TEntityDto, TKey, TGetListInput, TCreateInput, TCreateInput>
    where TEntityDto : class, IEntityDto<TKey>
{
}

