namespace WebAppEnum.Ebooks;

public class Ebook
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public BookType Type { get; set; }

    public DateTime PublishDate { get; set; }

    public float Price { get; set; }
}

public enum BookType
{
    Undefined,
    Adventure,
    Biography,
    Dystopia,
    Fantastic,
    Horror,
    Science,
    ScienceFiction,
    Poetry
}