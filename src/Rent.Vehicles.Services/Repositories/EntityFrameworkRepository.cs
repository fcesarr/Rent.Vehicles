using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

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

    public async Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var dbSet = _dbContext.Set<TEntity>();

        _ = await dbSet.AddAsync(entity, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var dbSet = _dbContext.Set<TEntity>();

        var entities = await dbSet
            .Where(predicate)
            .ToListAsync(cancellationToken);

        await Task.Run(() => dbSet.RemoveRange(entities), cancellationToken);
        
        await _dbContext.SaveChangesAsync(cancellationToken);    
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var dbSet = _dbContext.Set<TEntity>();

        await Task.Run(() => dbSet.Remove(entity), cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
        bool descending = false,
        Expression<Func<TEntity, dynamic>>? orderBy = default,
        IEnumerable<Expression<Func<TEntity, dynamic>>>? includes = default,
        CancellationToken cancellationToken = default)
    {
        var dbSet = _dbContext.Set<TEntity>()
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
        var dbSet = _dbContext.Set<TEntity>()
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

    public void SetContext(IDbContext context)
    {
        _dbContext = context;
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var dbSet = _dbContext.Set<TEntity>();

        await Task.Run(() => dbSet.Update(entity), cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
