using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Interfaces;

public interface IUpdateService<T> where T : Entity
{
    Task UpdateAsync(T? entity, CancellationToken cancellationToken = default);
}