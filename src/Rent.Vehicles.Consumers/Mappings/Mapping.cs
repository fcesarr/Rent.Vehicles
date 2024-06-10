using Rent.Vehicles.Entities;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Messages.Commands;

namespace Rent.Vehicles.Consumers.Mappings;

public static class Mapping
{
    public static async Task<H?> MapCreateVehiclesCommandToCommand<H>(this Message message, ISerializer serializer) where H : Entity
    {
        var data = message switch
        {
            CreateVehiclesCommand => await GetCreateVehiclesCommand(message, serializer),
            _ => await GetDefault(message, serializer)
        };

        return new Command
        {
            SagaId = message.SagaId,
            Type = Entities.Types.ActionType.Create,
            SerializerType = Lib.Types.SerializerType.MessagePack,
            Data = data
        } as H;
    }

    static async Task<byte[]> GetCreateVehiclesCommand(Message message, ISerializer serializer)
    {
        var command = message as CreateVehiclesCommand;

        if(command == null)
            return [];

        return await serializer.SerializeAsync(new { Id = command.Id });
    }

    static async Task<byte[]> GetDefault(Message message, ISerializer serializer)
    {
        return await serializer.SerializeAsync(new { Id = message.SagaId });
    }
}