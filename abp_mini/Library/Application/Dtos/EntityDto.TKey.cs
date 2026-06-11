namespace Library.Application.Dtos;

public abstract class EntityDto<TKey> : EntityDto, IEntityDto<TKey>
{
    public TKey Id { get; set; } = default!;
}
