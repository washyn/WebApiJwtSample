using Lib.Domain.Entities;

namespace App.Api.Entities;

public class Book : Entity<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
}