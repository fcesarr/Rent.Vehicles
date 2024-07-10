using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services.DataServices;

public class VehicleProjectionDataService : DataService<VehicleProjection>, IVehicleProjectionDataService
{
    public VehicleProjectionDataService(ILogger<VehicleProjectionDataService> logger,
        IValidator<VehicleProjection> validator, IRepository<VehicleProjection> repository) : base(logger, validator,
        repository)
    {
    }
}
