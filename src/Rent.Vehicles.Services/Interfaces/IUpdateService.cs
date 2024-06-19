using LanguageExt.Common;

using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Interfaces;

public interface IUpdateService<TEntity> where TEntity : Entity
{
    Task<Result<TEntity>> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
}