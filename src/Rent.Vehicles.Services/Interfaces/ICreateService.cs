using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Interfaces;

public interface ICreateService<T> where T : Entity
{
    Task CreateAsync(T? entity, CancellationToken cancellationToken = default);
}