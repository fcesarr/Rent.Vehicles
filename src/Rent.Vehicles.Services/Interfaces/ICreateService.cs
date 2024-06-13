using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Interfaces;

public interface ICreateService<TEntity> where TEntity : Entity
{
    Task CreateAsync(TEntity? entity, CancellationToken cancellationToken = default);    
}