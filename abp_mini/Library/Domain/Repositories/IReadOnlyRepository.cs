using System.Linq.Expressions;

using Lib.Domain.Entities;

namespace Lib.Domain.Repositories;

public interface IReadOnlyRepository<TEntity> : IReadOnlyBasicRepository<TEntity> where TEntity : class, IEntity
{
    Task<IQueryable<TEntity>> GetQueryableAsync();

    Task<IQueryable<TEntity>> WithDetailsAsync();

    Task<IQueryable<TEntity>> WithDetailsAsync(params Expression<Func<TEntity, object>>[] propertySelectors);

    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, bool includeDetails = false, CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting, bool includeDetails = false, CancellationToken cancellationToken = default);
    
    // Synchronous versions
    IQueryable<TEntity> GetQueryable();

    IQueryable<TEntity> WithDetails();

    IQueryable<TEntity> WithDetails(params Expression<Func<TEntity, object>>[] propertySelectors);

    List<TEntity> GetList(Expression<Func<TEntity, bool>> predicate, bool includeDetails = false);

    List<TEntity> GetPagedList(int skipCount, int maxResultCount, string sorting, bool includeDetails = false);
}

public interface IReadOnlyRepository<TEntity, TKey> : IReadOnlyRepository<TEntity>, IReadOnlyBasicRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
{
}
