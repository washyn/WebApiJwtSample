using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

using Library.Domain.Entities;
using Library.Domain.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Library.EntityFrameworkCore;

public class EfCoreRepository<TDbContext, TEntity> : IRepository<TEntity>
    where TDbContext : DbContext
    where TEntity : class, IEntity
{
    protected TDbContext DbContext { get; }

    public EfCoreRepository(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual DbSet<TEntity> DbSet => DbContext.Set<TEntity>();

    public virtual async Task<IQueryable<TEntity>> GetQueryableAsync()
    {
        return await Task.FromResult(DbSet.AsQueryable());
    }

    public virtual IQueryable<TEntity> GetQueryable()
    {
        return DbSet.AsQueryable();
    }

    public virtual async Task<IQueryable<TEntity>> WithDetailsAsync()
    {
        return await GetQueryableAsync();
    }

    public virtual IQueryable<TEntity> WithDetails()
    {
        return GetQueryable();
    }

    public virtual async Task<IQueryable<TEntity>> WithDetailsAsync(params Expression<Func<TEntity, object>>[] propertySelectors)
    {
        var query = await GetQueryableAsync();
        if (propertySelectors != null)
        {
            foreach (var propertySelector in propertySelectors)
            {
                query = query.Include(propertySelector);
            }
        }
        return query;
    }

    public virtual IQueryable<TEntity> WithDetails(params Expression<Func<TEntity, object>>[] propertySelectors)
    {
        var query = GetQueryable();
        if (propertySelectors != null)
        {
            foreach (var propertySelector in propertySelectors)
            {
                query = query.Include(propertySelector);
            }
        }
        return query;
    }

    public virtual async Task<List<TEntity>> GetListAsync(bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        return includeDetails
            ? await (await WithDetailsAsync()).ToListAsync(GetCancellationToken(cancellationToken))
            : await DbSet.ToListAsync(GetCancellationToken(cancellationToken));
    }

    public virtual List<TEntity> GetList(bool includeDetails = false)
    {
        return includeDetails
            ? WithDetails().ToList()
            : DbSet.ToList();
    }

    public virtual async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    public virtual long GetCount()
    {
        return DbSet.LongCount();
    }

    public virtual async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = includeDetails ? await WithDetailsAsync() : await GetQueryableAsync();
        return await query.Where(predicate).ToListAsync(GetCancellationToken(cancellationToken));
    }

    public virtual List<TEntity> GetList(Expression<Func<TEntity, bool>> predicate, bool includeDetails = false)
    {
        var query = includeDetails ? WithDetails() : GetQueryable();
        return query.Where(predicate).ToList();
    }

    public virtual async Task<List<TEntity>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = includeDetails ? await WithDetailsAsync() : await GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(sorting))
        {
            query = query.OrderBy(sorting);
        }

        return await query.Skip(skipCount).Take(maxResultCount).ToListAsync(GetCancellationToken(cancellationToken));
    }

    public virtual List<TEntity> GetPagedList(int skipCount, int maxResultCount, string sorting, bool includeDetails = false)
    {
        var query = includeDetails ? WithDetails() : GetQueryable();

        if (!string.IsNullOrWhiteSpace(sorting))
        {
            query = query.OrderBy(sorting);
        }

        return query.Skip(skipCount).Take(maxResultCount).ToList();
    }

    public virtual async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var savedEntity = (await DbContext.Set<TEntity>().AddAsync(entity, GetCancellationToken(cancellationToken))).Entity;

        if (autoSave)
        {
            await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
        }

        return savedEntity;
    }

    public virtual TEntity Insert(TEntity entity, bool autoSave = false)
    {
        var savedEntity = DbContext.Set<TEntity>().Add(entity).Entity;

        if (autoSave)
        {
            DbContext.SaveChanges();
        }

        return savedEntity;
    }

    public virtual async Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var entityArray = entities.ToArray();
        await DbContext.Set<TEntity>().AddRangeAsync(entityArray, GetCancellationToken(cancellationToken));

        if (autoSave)
        {
            await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
        }
    }

    public virtual void InsertMany(IEnumerable<TEntity> entities, bool autoSave = false)
    {
        var entityArray = entities.ToArray();
        DbContext.Set<TEntity>().AddRange(entityArray);

        if (autoSave)
        {
            DbContext.SaveChanges();
        }
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        DbContext.Attach(entity);
        var updatedEntity = DbContext.Update(entity).Entity;

        if (autoSave)
        {
            await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
        }

        return updatedEntity;
    }

    public virtual TEntity Update(TEntity entity, bool autoSave = false)
    {
        DbContext.Attach(entity);
        var updatedEntity = DbContext.Update(entity).Entity;

        if (autoSave)
        {
            DbContext.SaveChanges();
        }

        return updatedEntity;
    }

    public virtual async Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var entityArray = entities.ToArray();
        DbContext.Set<TEntity>().UpdateRange(entityArray);

        if (autoSave)
        {
            await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
        }
    }

    public virtual void UpdateMany(IEnumerable<TEntity> entities, bool autoSave = false)
    {
        var entityArray = entities.ToArray();
        DbContext.Set<TEntity>().UpdateRange(entityArray);

        if (autoSave)
        {
            DbContext.SaveChanges();
        }
    }

    public virtual async Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        DbContext.Set<TEntity>().Remove(entity);

        if (autoSave)
        {
            await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
        }
    }

    public virtual void Delete(TEntity entity, bool autoSave = false)
    {
        DbContext.Set<TEntity>().Remove(entity);

        if (autoSave)
        {
            DbContext.SaveChanges();
        }
    }

    public virtual async Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        DbContext.RemoveRange(entities);

        if (autoSave)
        {
            await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
        }
    }

    public virtual void DeleteMany(IEnumerable<TEntity> entities, bool autoSave = false)
    {
        DbContext.RemoveRange(entities);

        if (autoSave)
        {
            DbContext.SaveChanges();
        }
    }

    protected virtual CancellationToken GetCancellationToken(CancellationToken preferredValue = default)
    {
        return preferredValue;
    }
}

public class EfCoreRepository<TDbContext, TEntity, TKey> : EfCoreRepository<TDbContext, TEntity>, IRepository<TEntity, TKey>
    where TDbContext : DbContext
    where TEntity : class, IEntity<TKey>
{
    public EfCoreRepository(TDbContext dbContext) : base(dbContext)
    {
    }

    public virtual async Task<TEntity> GetAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(id, includeDetails, cancellationToken);

        if (entity == null)
        {
            throw new Exception($"Entity of type {typeof(TEntity).Name} with Id {id} not found.");
        }

        return entity;
    }

    public virtual TEntity Get(TKey id, bool includeDetails = true)
    {
        var entity = Find(id, includeDetails);

        if (entity == null)
        {
            throw new Exception($"Entity of type {typeof(TEntity).Name} with Id {id} not found.");
        }

        return entity;
    }

    public virtual async Task<TEntity?> FindAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        return includeDetails
            ? await (await WithDetailsAsync()).FirstOrDefaultAsync(e => e.Id!.Equals(id), GetCancellationToken(cancellationToken))
            : await DbSet.FindAsync(new object[] { id! }, GetCancellationToken(cancellationToken));
    }

    public virtual TEntity? Find(TKey id, bool includeDetails = true)
    {
        return includeDetails
            ? WithDetails().FirstOrDefault(e => e.Id!.Equals(id))
            : DbSet.Find(id);
    }

    public virtual async Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(id, cancellationToken: cancellationToken);
        if (entity == null)
        {
            return;
        }

        await DeleteAsync(entity, autoSave, cancellationToken);
    }

    public virtual void Delete(TKey id, bool autoSave = false)
    {
        var entity = Find(id);
        if (entity == null)
        {
            return;
        }

        Delete(entity, autoSave);
    }

    public virtual async Task DeleteManyAsync(IEnumerable<TKey> ids, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var entities = await DbSet.Where(e => ids.Contains(e.Id)).ToListAsync(GetCancellationToken(cancellationToken));

        await DeleteManyAsync(entities, autoSave, cancellationToken);
    }

    public virtual void DeleteMany(IEnumerable<TKey> ids, bool autoSave = false)
    {
        var entities = DbSet.Where(e => ids.Contains(e.Id)).ToList();

        DeleteMany(entities, autoSave);
    }
}
