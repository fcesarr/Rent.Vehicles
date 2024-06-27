
using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services.DataServices;

public class UserProjectionDataService : DataService<UserProjection>, IUserProjectionDataService
{
    public UserProjectionDataService(ILogger<DataService<UserProjection>> logger,
        IValidator<UserProjection> validator,
        IRepository<UserProjection> repository) : base(logger, validator, repository)
    {
    }
}