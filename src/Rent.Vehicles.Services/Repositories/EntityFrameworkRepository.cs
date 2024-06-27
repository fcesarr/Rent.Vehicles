using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Contexts.Interfaces;
using Rent.Vehicles.Entities.Factories.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;

namespace Rent.Vehicles.Services.Repositories;

public sealed class EntityFrameworkRepository<TEntity> : IRepository, IRepository<TEntity> where TEntity : Entity
{
    private IDbContext _dbContext;

    public EntityFrameworkRepository(IDbContextFactory dbContextFactory)
    {
        _dbContext = dbContextFactory.CreateDbContextAsync().GetAwaiter().GetResult();
    }

    public void SetContext(IDbContext context)
    {
        _dbContext = context;
    }

    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbSet<TEntity> dbSet = _dbContext.Set<TEntity>();

        EntityEntry<TEntity> entityEntry = await dbSet.AddAsync(entity, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return entityEntry.Entity;
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbSet<TEntity> dbSet = _dbContext.Set<TEntity>();

        await Task.Run(() => dbSet.Remove(entity), cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
        bool descending = false,
        Expression<Func<TEntity, dynamic>>? orderBy = default,
        IEnumerable<Expression<Func<TEntity, dynamic>>>? includes = default,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> dbSet = _dbContext.Set<TEntity>()
            .Where(predicate);

        if (orderBy is not null)
        {
            dbSet = descending ? dbSet.OrderByDescending(orderBy) : dbSet.OrderBy(orderBy);
        }

        if (includes is not null)
        {
            dbSet = includes
                .Aggregate(dbSet, (current, include) => current.Include(include));
        }

        return await dbSet
            .ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate,
        bool descending = false,
        Expression<Func<TEntity, dynamic>>? orderBy = default,
        IEnumerable<Expression<Func<TEntity, dynamic>>>? includes = default,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> dbSet = _dbContext.Set<TEntity>()
            .Where(predicate);

        if (orderBy is not null)
        {
            dbSet = descending ? dbSet.OrderByDescending(orderBy) : dbSet.OrderBy(orderBy);
        }

        if (includes is not null)
        {
            dbSet = includes
                .Aggregate(dbSet, (current, include) => current.Include(include));
        }

        return await dbSet
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbSet<TEntity> dbSet = _dbContext.Set<TEntity>();

        EntityEntry<TEntity> entityEntry = await Task.Run(() => dbSet.Update(entity), cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return entityEntry.Entity;
    }

    public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        DbSet<TEntity> dbSet = _dbContext.Set<TEntity>();

        List<TEntity> entities = await dbSet
            .Where(predicate)
            .ToListAsync(cancellationToken);

        await Task.Run(() => dbSet.RemoveRange(entities), cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}