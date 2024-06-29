using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Services.DataServices.Interfaces;

public interface IRentDataService : IDataService<Entities.Rent>
{
    Task<Result<Entities.Rent>> UpdateAsync(Guid id, DateTime estimatedDate,
        CancellationToken cancellationToken = default);

    Task<Result<Entities.Rent>> CreateAsync(RentalPlane rentalPlane, Guid id, Guid userId, Guid vehicleId,
        CancellationToken cancellationToken = default);

    Task<Result<Entities.Rent>> EstimateCostAsync(Guid id, DateTime estimatedDate,
        CancellationToken cancellationToken = default);
}
