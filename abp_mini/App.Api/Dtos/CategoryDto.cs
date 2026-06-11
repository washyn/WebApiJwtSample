using System;
using MyLibrary.Application.Dtos;

namespace MyApp.Api.Dtos;

public class CategoryDto : EntityDto<Guid>
{
    public string Name { get; set; } = string.Empty;
}
