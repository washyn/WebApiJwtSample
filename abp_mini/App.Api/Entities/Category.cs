using Lib.Domain.Entities;

namespace App.Api.Entities;

public class Category : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;
}