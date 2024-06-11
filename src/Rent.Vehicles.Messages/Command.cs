using MessagePack;

namespace Rent.Vehicles.Messages;

[MessagePackObject]
public record Command : Message
{
    public Command()
    {
        SagaId = Guid.NewGuid();
    }
}