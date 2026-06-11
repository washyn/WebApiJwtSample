namespace App.Api.Dtos;

public class CreateUpdateBookDto
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
}
