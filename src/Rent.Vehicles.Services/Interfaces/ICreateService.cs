using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Interfaces;

public interface ICreateService<in T> where T : Entity
{
    Task Create(T? entity, CancellationToken cancellationToken = default);
}