using Lib.Domain.Entities;

namespace App.Api.Entities;

public class TodoItem : Entity<Guid>
{
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}