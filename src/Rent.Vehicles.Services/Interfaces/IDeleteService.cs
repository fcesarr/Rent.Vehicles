using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Interfaces;

public interface IDeleteService<T> where T : Entity
{
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}