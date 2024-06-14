using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services;

public class VehicleService : NoSqlService<Vehicle>, IVehicleService
{
    public VehicleService(ILogger<Service<Vehicle>> logger,
        IValidator<Vehicle> validator,
        IRepository<Vehicle> repository) : base(logger, validator, repository)
    {
    }

    public async Task UpdateAsync(Guid id, string licensePlate, CancellationToken cancellationToken = default)
    {
        var entity = await GetAsync(x => x.Id == id, cancellationToken);

        if(entity is null)
            return;

        entity.LicensePlate = licensePlate;

        await UpdateAsync(entity, cancellationToken);
    }
}

public interface IVehicleService : IService<Vehicle>
{
    Task UpdateAsync(Guid id, string licensePlate, CancellationToken cancellationToken = default);
}
