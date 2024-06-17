using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Services;

public class VehiclesService : Service<Vehicle>, IVehiclesService
{
    public VehiclesService(ILogger<Service<Vehicle>> logger,
        IValidator<Vehicle> validator,
        ISqlRepository<Vehicle> repository) : base(logger, validator, repository)
    {
    }

    public VehiclesService(ILogger<Service<Vehicle>> logger,
        IValidator<Vehicle> validator,
        INoSqlRepository<Vehicle> repository) : base(logger, validator, repository)
    {
    }

    public async Task UpdateAsync(Guid id, string licensePlate, CancellationToken cancellationToken = default)
    {
        var entity = await GetAsync(x => x.Id == id, cancellationToken);

        if(entity == null)
            return;
        
        entity.LicensePlate = licensePlate;

        await UpdateAsync(entity, cancellationToken);
    }
}