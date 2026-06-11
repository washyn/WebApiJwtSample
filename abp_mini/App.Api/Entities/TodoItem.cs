using System;
using MyLibrary.Domain.Entities;

namespace MyApp.Api.Entities;

public class TodoItem : Entity<Guid>
{
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}
