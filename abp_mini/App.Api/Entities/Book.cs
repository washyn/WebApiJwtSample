using System;
using MyLibrary.Domain.Entities;

namespace MyApp.Api.Entities;

public class Book : Entity<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
}
