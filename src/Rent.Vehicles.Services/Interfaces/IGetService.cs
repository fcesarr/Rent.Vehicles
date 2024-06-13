using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Interfaces;

public interface IGetService<T> where T : Entity
{
    Task<T?> GetAsync(Guid id, CancellationToken cancellationToken = default);
}