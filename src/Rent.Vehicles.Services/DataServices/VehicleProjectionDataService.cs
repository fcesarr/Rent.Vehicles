
using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Exceptions;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services.DataServices;

public class VehicleProjectionDataService : DataService<VehicleProjection>, IVehicleProjectionDataService
{
    public VehicleProjectionDataService(ILogger<VehicleProjectionDataService> logger,
        IValidator<VehicleProjection> validator,
        IRepository<VehicleProjection> repository) : base(logger, validator, repository)
    {
    }

    public async Task<Result<VehicleProjection>> UpdateAsync(Guid id,
        string licensePlate,
        CancellationToken cancellationToken = default)
    {
        var entity = await GetAsync(x => x.Id == id, cancellationToken);

        if(!entity.IsSuccess)
            return Result<VehicleProjection>.Failure(entity.Exception);

        if(entity.Value is null)
            return Result<VehicleProjection>.Failure(new NullException("Vehicle not found."));

        return await UpdateAsync(entity.Value, cancellationToken);
    }
}