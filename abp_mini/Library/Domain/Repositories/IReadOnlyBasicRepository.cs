using Lib.Domain.Entities;

namespace Lib.Domain.Repositories;

public interface IReadOnlyBasicRepository<TEntity> where TEntity : class, IEntity
{
    Task<List<TEntity>> GetListAsync(bool includeDetails = false, CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(CancellationToken cancellationToken = default);
    
    // Synchronous versions
    List<TEntity> GetList(bool includeDetails = false);
    long GetCount();
}

public interface IReadOnlyBasicRepository<TEntity, TKey> : IReadOnlyBasicRepository<TEntity> where TEntity : class, IEntity<TKey>
{
    Task<TEntity> GetAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default);
    
    Task<TEntity?> FindAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default);
    
    // Synchronous versions
    TEntity Get(TKey id, bool includeDetails = true);
    
    TEntity? Find(TKey id, bool includeDetails = true);
}
