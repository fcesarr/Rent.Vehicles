using System.Linq.Expressions;

using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Interfaces;

public interface IDeleteService<TEntity> where TEntity : Entity
{
    Task DeleteAsync(TEntity? entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}