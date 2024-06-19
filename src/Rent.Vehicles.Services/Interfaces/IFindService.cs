using System.Linq.Expressions;

using LanguageExt.Common;

using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Interfaces;

public interface IFindService<TEntity> where TEntity : Entity
{
    Task<Result<IEnumerable<TEntity>>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
}