using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services.DataServices;

public class RentProjectionDataService : DataService<RentProjection>, IRentProjectionDataService
{
    public RentProjectionDataService(ILogger<RentProjectionDataService> logger,
        IValidator<RentProjection> validator,
        IRepository<RentProjection> repository) : base(logger, validator, repository)
    {
    }
}
