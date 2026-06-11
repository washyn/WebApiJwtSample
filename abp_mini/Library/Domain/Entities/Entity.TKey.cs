namespace Library.Domain.Entities;

/// <summary>
/// Basic implementation of IEntity interface.
/// An entity can inherit this class of directly implement to IEntity interface.
/// </summary>
/// <typeparam name="TKey">Type of the primary key of the entity</typeparam>
public abstract class Entity<TKey> : Entity, IEntity<TKey>
{
    /// <summary>
    /// Unique identifier for this entity.
    /// </summary>
    public virtual TKey Id { get; set; } = default!;

    public override object[] GetKeys()
    {
        return [Id!];
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"[Entity: {GetType().Name}] Id = {Id}";
    }
}
