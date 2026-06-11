namespace MyLibrary.Domain.Entities;

/// <summary>
/// Basic implementation of IEntity interface.
/// An entity can inherit this class of directly implement to IEntity interface.
/// </summary>
public abstract class Entity : IEntity
{
    public abstract object[] GetKeys();

    public override string ToString()
    {
        return $"[Entity: {GetType().Name}] Keys = {string.Join(", ", GetKeys())}";
    }
}
