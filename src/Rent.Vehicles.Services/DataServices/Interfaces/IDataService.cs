using System.Linq.Expressions;

using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Interfaces;

public interface IDataService<TEntity> where TEntity : Entity
{
    Task<Result<TEntity>> CreateAsync(TEntity? entity, CancellationToken cancellationToken = default);

    Task<Result<bool>> DeleteAsync(TEntity? entity, CancellationToken cancellationToken = default);

    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<TEntity>>> FindAsync(Expression<Func<TEntity, bool>> predicate,
        bool descending = false,
        Expression<Func<TEntity, dynamic>>? orderBy = default,
        CancellationToken cancellationToken = default);

    Task<Result<TEntity>> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<Result<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
}