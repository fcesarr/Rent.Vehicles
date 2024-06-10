using Rent.Vehicles.Entities;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Messages.Commands;

namespace Rent.Vehicles.Consumers.Mappings;

public static class Mapping
{
    public static async Task<H?> MapCommandToCommand<H>(this Message message, ISerializer serializer) where H : Entity
    {
        var data = message switch
        {
            CreateVehiclesCommand => new { @Type = Entities.Types.ActionType.Create, Data = await GetCreateVehiclesCommand(message, serializer) },
            DeleteVehiclesCommand => new { @Type = Entities.Types.ActionType.Delete, Data = await GetDeleteVehiclesCommand(message, serializer) },
            _ => new { @Type = Entities.Types.ActionType.Create, Data = await GetDefault(message, serializer) }
        };

        return new Command
        {
            SagaId = message.SagaId,
            Type = data.Type,
            SerializerType = Lib.Types.SerializerType.MessagePack,
            Data = data.Data
        } as H;
    }

    static async Task<byte[]> GetCreateVehiclesCommand(Message message, ISerializer serializer)
    {
        var command = message as CreateVehiclesCommand;

        if(command == null)
            return [];

        return await serializer.SerializeAsync(new { Id = command.Id });
    }

    static async Task<byte[]> GetDeleteVehiclesCommand(Message message, ISerializer serializer)
    {
        var command = message as DeleteVehiclesCommand;

        if(command == null)
            return [];

        return await serializer.SerializeAsync(new { Id = command.Id });
    }

    static async Task<byte[]> GetDefault(Message message, ISerializer serializer)
    {
        return await serializer.SerializeAsync(new { Id = message.SagaId });
    }
}