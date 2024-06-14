using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Interfaces;

public interface IUpdateService<TEntity> where TEntity : Entity
{
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task UpdateAsync<TField>(TField field, CancellationToken cancellationToken = default);
}