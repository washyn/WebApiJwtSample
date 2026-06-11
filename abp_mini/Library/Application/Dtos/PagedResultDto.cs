using System.Collections.Generic;

namespace MyLibrary.Application.Dtos;

public class PagedResultDto<T>
{
    public long TotalCount { get; set; }
    
    public IReadOnlyList<T> Items { get; set; }

    public PagedResultDto()
    {
        Items = new List<T>();
    }

    public PagedResultDto(long totalCount, IReadOnlyList<T> items)
    {
        TotalCount = totalCount;
        Items = items;
    }
}
