using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Exceptions;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services;

public class VehicleDataService : DataService<Vehicle>, IVehicleDataService
{
    public VehicleDataService(ILogger<VehicleDataService> logger,
        IVehicleValidator validator,
        IRepository<Vehicle> repository) : base(logger, validator, repository)
    {
    }

    public async Task<Result<Vehicle>> UpdateAsync(Guid id,
        string licensePlate,
        CancellationToken cancellationToken = default)
    {
        Result<Vehicle> entity = await GetAsync(x => x.Id == id, cancellationToken);

        if (!entity.IsSuccess)
        {
            return Result<Vehicle>.Failure(entity.Exception);
        }

        if (entity.Value is null)
        {
            return Result<Vehicle>.Failure(new NullException("Vehicle not found."));
        }


        return await UpdateAsync(entity.Value, cancellationToken);
    }
}