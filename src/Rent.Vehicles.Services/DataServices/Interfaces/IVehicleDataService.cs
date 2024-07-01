using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Interfaces;

public interface IVehicleDataService : IDataService<Vehicle>
{
    Task<Result<Vehicle>> UpdateAsync(Guid id,
        string licensePlate,
        CancellationToken cancellationToken = default);
    
    Task<Result<Vehicle>> RentItAsync(CancellationToken cancellationToken = default);

    Task<Result<Vehicle>> ReturnItAsync(Guid id, CancellationToken cancellationToken = default);
}
