namespace Library.Application.Dtos;

public interface IEntityDto<TKey> : IEntityDto
{
    TKey Id { get; set; }
}
