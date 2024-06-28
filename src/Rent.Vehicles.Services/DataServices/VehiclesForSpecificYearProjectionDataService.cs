using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services.DataServices;

public class VehiclesForSpecificYearProjectionDataService : DataService<VehiclesForSpecificYearProjection>, IVehiclesForSpecificYearProjectionDataService
{
    public VehiclesForSpecificYearProjectionDataService(ILogger<VehiclesForSpecificYearProjectionDataService> logger, IValidator<VehiclesForSpecificYearProjection> validator, IRepository<VehiclesForSpecificYearProjection> repository) : base(logger, validator, repository)
    {
    }
}
