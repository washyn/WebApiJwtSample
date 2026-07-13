namespace Lib.Application.Dtos;

public class PagedAndSortedResultRequestDto
{
    public int SkipCount { get; set; } = 0;
    
    public int MaxResultCount { get; set; } = 10;
    
    public string Sorting { get; set; } = string.Empty;
}
