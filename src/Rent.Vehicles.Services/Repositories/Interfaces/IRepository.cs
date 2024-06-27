using System.Linq.Expressions;

using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Repositories.Interfaces;

public interface IRepository<TEntity> where TEntity : Entity
{
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate,
        bool descending = false,
        Expression<Func<TEntity, dynamic>>? orderBy = default,
        IEnumerable<Expression<Func<TEntity, dynamic>>>? includes = default,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
        bool descending = false,
        Expression<Func<TEntity, dynamic>>? orderBy = default,
        IEnumerable<Expression<Func<TEntity, dynamic>>>? includes = default,
        CancellationToken cancellationToken = default);
}
