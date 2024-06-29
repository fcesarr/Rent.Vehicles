using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services.DataServices;

public class EventProjectionDataService : DataService<EventProjection>, IEventProjectionDataService
{
    public EventProjectionDataService(ILogger<EventProjectionDataService> logger,
        IValidator<EventProjection> validator,
        IRepository<EventProjection> repository) : base(logger, validator, repository)
    {
    }
}
