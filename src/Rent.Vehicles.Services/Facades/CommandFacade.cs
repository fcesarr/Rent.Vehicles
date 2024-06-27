using Rent.Vehicles.Entities.Types;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Lib.Types;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Extensions;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades;

public class CommandFacade : ICommandFacade
{
    private readonly ICommandDataService _dataService;

    private readonly ISerializer _serializer;

    public CommandFacade(ICommandDataService dataService, ISerializer serializer)
    {
        _dataService = dataService;
        _serializer = serializer;
    }

    public async Task<Result<CommandResponse>> CreateAsync(Command command,
        Event @event,
        ActionType actionType,
        SerializerType serializerType,
        EntityType entityType,
        string type,
        CancellationToken cancellationToken = default)
    {
        var data = await _serializer.SerializeAsync(@event, @event.GetType(), cancellationToken);

        var entity =
            await _dataService.CreateAsync(command.ToEntity(actionType, serializerType, entityType, type, data),
                cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return entity.Value!.ToResponse();
    }
}
