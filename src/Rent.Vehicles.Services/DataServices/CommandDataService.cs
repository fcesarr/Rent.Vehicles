using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services.DataServices;

public class CommandDataService : DataService<Command>, ICommandDataService
{
    public CommandDataService(ILogger<DataService<Command>> logger,
        IValidator<Command> validator,
        IRepository<Command> repository) : base(logger, validator, repository)
    {
    }
}