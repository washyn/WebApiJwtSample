using Library.Application.Dtos;

namespace App.Api.Dtos;

public class BookDto : EntityDto<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
}