using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services.DataServices;

public class RentalPlaneDataService : DataService<RentalPlane>, IRentalPlaneDataService
{
    public RentalPlaneDataService(ILogger<RentalPlaneDataService> logger, IRentalPlaneValidator validator, IRepository<RentalPlane> repository) : base(logger, validator, repository)
    {
    }
}
