using Lib.Application.Dtos;

namespace Lib.Application.Services;

public interface IReadOnlyAppService<TEntityDto, in TKey, in TGetListInput> : IApplicationService
    where TEntityDto : class, IEntityDto<TKey>
{
    Task<TEntityDto> GetAsync(TKey id);

    Task<PagedResultDto<TEntityDto>> GetListAsync(TGetListInput input);
}

public interface IReadOnlyAppService<TEntityDto, TKey>
    : IReadOnlyAppService<TEntityDto, TKey, PagedAndSortedResultRequestDto>
    where TEntityDto : class, IEntityDto<TKey>
{
}

