using Library.Domain.Entities;
using Library.EntityFrameworkCore;

namespace App.Api.Data;

public class AppDbContextRepository<TEntity, TKey> : EfCoreRepository<AppDbContext, TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    public AppDbContextRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
