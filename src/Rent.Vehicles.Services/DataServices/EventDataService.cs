using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services.DataServices;

public class EventDataService : DataService<Entities.Event>, IEventDataService
{
    public EventDataService(ILogger<EventDataService> logger, IEventValidator validator, IRepository<Event> repository) : base(logger, validator, repository)
    {
    }
}
