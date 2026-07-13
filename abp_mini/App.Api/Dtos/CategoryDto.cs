using Library.Application.Dtos;

namespace App.Api.Dtos;

public class CategoryDto : EntityDto<Guid>
{
    public string Name { get; set; } = string.Empty;
}