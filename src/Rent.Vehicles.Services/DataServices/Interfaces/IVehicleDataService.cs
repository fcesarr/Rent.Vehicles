
using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Projections;

namespace Rent.Vehicles.Services.Interfaces;

public interface IVehicleDataService : IVehicleDataService<Vehicle>;

public interface IVehicleDataService<TEntity> : IDataService<TEntity> where TEntity : Entity
{
    Task<Result<TEntity>> UpdateAsync(Guid id,
        string licensePlate,
        CancellationToken cancellationToken = default);
}