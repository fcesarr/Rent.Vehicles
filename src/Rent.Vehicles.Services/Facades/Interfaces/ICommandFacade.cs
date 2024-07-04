using Rent.Vehicles.Entities.Types;
using Rent.Vehicles.Lib;
using Rent.Vehicles.Lib.Types;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades.Interfaces;

public interface ICommandFacade : IFacade
{
    Task<Result<CommandResponse>> CreateAsync(Command command,
        Event @event,
        ActionType actionType,
        EntityType entityType,
        string type,
        CancellationToken cancellationToken = default);
}
