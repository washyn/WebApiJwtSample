using Library.Domain.Entities;
using Library.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;

namespace App.Api.Data;

public class AppDbContextRepository<TEntity, TKey> : EfCoreRepository<DbContext, TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    public AppDbContextRepository(DbContext dbContext) : base(dbContext)
    {
    }
}
