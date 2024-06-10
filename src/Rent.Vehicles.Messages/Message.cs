using MessagePack;

namespace Rent.Vehicles.Messages;

[MessagePackObject]
public abstract record Message
{
    [Key(0)]
    public Guid Id { get; init; } = Guid.NewGuid();
}