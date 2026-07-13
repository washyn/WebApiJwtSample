using Lib.Domain.Entities;

namespace Lib.Domain.Repositories;

public interface IBasicRepository<TEntity> : IReadOnlyBasicRepository<TEntity> where TEntity : class, IEntity
{
    Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

    Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);

    Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

    Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);

    Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

    Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);
    
    // Synchronous versions
    TEntity Insert(TEntity entity, bool autoSave = false);

    void InsertMany(IEnumerable<TEntity> entities, bool autoSave = false);

    TEntity Update(TEntity entity, bool autoSave = false);

    void UpdateMany(IEnumerable<TEntity> entities, bool autoSave = false);

    void Delete(TEntity entity, bool autoSave = false);

    void DeleteMany(IEnumerable<TEntity> entities, bool autoSave = false);
}

public interface IBasicRepository<TEntity, TKey> : IBasicRepository<TEntity>, IReadOnlyBasicRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
{
    Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default);

    Task DeleteManyAsync(IEnumerable<TKey> ids, bool autoSave = false, CancellationToken cancellationToken = default);
    
    // Synchronous versions
    void Delete(TKey id, bool autoSave = false);

    void DeleteMany(IEnumerable<TKey> ids, bool autoSave = false);
}
