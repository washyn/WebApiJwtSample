using Lib.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Lib.EntityFrameworkCore;

public class DbContextRepository<TEntity, TKey> : EfCoreRepository<DbContext, TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    public DbContextRepository(DbContext dbContext) : base(dbContext)
    {
    }
}
