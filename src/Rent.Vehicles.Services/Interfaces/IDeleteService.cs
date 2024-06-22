using System.Linq.Expressions;

using LanguageExt.Common;

using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Interfaces;

public interface IDeleteService<TEntity> where TEntity : class
{
    Task<Result<bool>> DeleteAsync(TEntity? entity, CancellationToken cancellationToken = default);

    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}