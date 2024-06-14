using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services;

public class VehicleService : NoSqlService<Vehicle>, IVehicleService
{
    public VehicleService(ILogger<Service<Vehicle>> logger, IValidator<Vehicle> validator, IRepository<Vehicle> repository) : base(logger, validator, repository)
    {
    }

    public override async Task UpdateAsync<TField>(TField field, CancellationToken cancellationToken = default)
    {
        if(field is VehicleUpdate vehicleUpdate)
        {
            _logger.LogInformation("VehicleUpdate");

            var entity = await GetAsync(x => x.Id == vehicleUpdate.Id, cancellationToken);

            if(entity is null)
                return;

            _logger.LogInformation("Entity is not null");

            entity.LicensePlate = vehicleUpdate.LicensePlate;

            await UpdateAsync(entity, cancellationToken);

           _logger.LogInformation($"Update {entity}");
        }

    }

    public async Task UpdateAsync(Guid id, string licensePlate, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("VehicleUpdate 1");

        var entity = await GetAsync(x => x.Id == id, cancellationToken);

        if(entity is null)
                return;

            _logger.LogInformation("Entity is not null 1");

            entity.LicensePlate = licensePlate;

            await UpdateAsync(entity, cancellationToken);

           _logger.LogInformation("Update 1");
    }
}

public interface IVehicleService : IService<Vehicle>
{
    Task UpdateAsync(Guid id, string licensePlate, CancellationToken cancellationToken = default);
}

public class VehicleUpdate : Entity
{
    public string LicensePlate { get; set; }
}