using LanguageExt.Common;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Projections;

namespace Rent.Vehicles.Services.Interfaces;

public interface IVehicleService : IVehiclesService<Vehicle>;

public interface IVehicleProjectionService : IVehiclesService<VehicleProjection>;

public interface IVehiclesService<TEntity> : IService<TEntity> where TEntity : Entity
{
    Task<Result<TEntity>> UpdateAsync(Guid id,
        string licensePlate,
        CancellationToken cancellationToken = default);
}