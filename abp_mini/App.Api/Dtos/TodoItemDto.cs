using Lib.Application.Dtos;

namespace App.Api.Dtos;

public class TodoItemDto : EntityDto<Guid>
{
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}