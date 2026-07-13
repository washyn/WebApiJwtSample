using Lib.Domain.Entities;

namespace Lib.Domain.Repositories;

public interface IRepository<TEntity> : IReadOnlyRepository<TEntity>, IBasicRepository<TEntity> where TEntity : class, IEntity
{
}

public interface IRepository<TEntity, TKey> : IRepository<TEntity>, IReadOnlyRepository<TEntity, TKey>, IBasicRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
{
}
