using Library.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Library.EntityFrameworkCore;

public class DbContextRepository<TEntity, TKey> : EfCoreRepository<DbContext, TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    public DbContextRepository(DbContext dbContext) : base(dbContext)
    {
    }
}
