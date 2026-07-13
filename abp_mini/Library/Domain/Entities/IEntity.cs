namespace Lib.Domain.Entities;

/// <summary>
/// Defines an entity. It's primary key may not be "Id" or it may have a composite primary key.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Returns an array of ordered keys for this entity.
    /// </summary>
    /// <returns></returns>
    object[] GetKeys();
}
