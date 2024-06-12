using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Repositories.Interfaces;

public interface IMongoRepository<T> where T : Entity
{
    Task CreateAsync(T entity, CancellationToken cancellationToken = default);
}