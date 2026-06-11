using System;
using MyLibrary.Application.Dtos;

namespace MyApp.Api.Dtos;

public class TodoItemDto : EntityDto<Guid>
{
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}
