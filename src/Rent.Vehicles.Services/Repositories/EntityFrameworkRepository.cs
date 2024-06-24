using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Contexts.Interfaces;
using Rent.Vehicles.Entities.Factories.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;

namespace Rent.Vehicles.Services.Repositories;

public sealed class EntityFrameworkRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    private readonly IDbContextFactory _dbContextFactory;

    public EntityFrameworkRepository(IDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        using var context = await _dbContextFactory
            .CreateDbContextAsync(cancellationToken);

        var dbSet = context.Set<TEntity>();

        _ = await dbSet.AddAsync(entity, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        using var context = await _dbContextFactory
            .CreateDbContextAsync(cancellationToken);

        var dbSet = context.Set<TEntity>();

        var entities = await dbSet
            .Where(predicate)
            .ToListAsync(cancellationToken);

        await Task.Run(() => dbSet.RemoveRange(entities), cancellationToken);
        
        await context.SaveChangesAsync(cancellationToken);    
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        using var context = await _dbContextFactory
            .CreateDbContextAsync(cancellationToken);

        var dbSet = context.Set<TEntity>();

        await Task.Run(() => dbSet.Remove(entity), cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
        bool descending = false,
        Expression<Func<TEntity, dynamic>>? orderBy = default,
        CancellationToken cancellationToken = default)
    {
        using var context = await _dbContextFactory
            .CreateDbContextAsync(cancellationToken);

        var dbSet = context.Set<TEntity>()
            .Where(predicate);

        if (orderBy is not null)
		{
			dbSet = descending ? dbSet.OrderByDescending(orderBy) : dbSet.OrderBy(orderBy);
		}

        return await dbSet
            .ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        using var context = await _dbContextFactory
            .CreateDbContextAsync(cancellationToken);

        var dbSet = context.Set<TEntity>();

        return await dbSet
            .Where(predicate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
         using var context = await _dbContextFactory
            .CreateDbContextAsync(cancellationToken);

        var dbSet = context.Set<TEntity>();

        await Task.Run(() => dbSet.Update(entity), cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }
}
