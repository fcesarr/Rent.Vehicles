using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using LanguageExt.Common;
using Rent.Vehicles.Services.Exceptions;

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
        var entity = await GetAsync(x => x.Id == id, cancellationToken);

        return await entity.Match(async entity => 
        {
            if(entity == null)
                return new Result<Vehicle>(new NullException());

            entity.LicensePlate = licensePlate;

            return await UpdateAsync(entity, cancellationToken);
        }, exception => Task.FromResult(new Result<Vehicle>(exception)));
    }
}
