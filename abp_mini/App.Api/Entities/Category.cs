using System;
using MyLibrary.Domain.Entities;

namespace MyApp.Api.Entities;

public class Category : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;
}
