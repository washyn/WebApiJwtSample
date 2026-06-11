using System;
using MyLibrary.Application.Dtos;

namespace MyApp.Api.Dtos;

public class BookDto : EntityDto<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
}
