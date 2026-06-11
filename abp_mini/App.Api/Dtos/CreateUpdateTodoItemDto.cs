namespace MyApp.Api.Dtos;

public class CreateUpdateTodoItemDto
{
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}
