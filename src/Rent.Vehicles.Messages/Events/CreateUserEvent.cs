using MessagePack;

using Rent.Vehicles.Messages.Types;

namespace Rent.Vehicles.Messages.Events;

[MessagePackObject]
public record CreateUserEvent : Messages.Event
{
    [Key(1)]
    public required Guid Id { get; init; }

    [Key(2)]
    public required string Name { get; set; }

    [Key(3)]
    public required string Number { get; set; }

    [Key(4)]
    public required DateTime Birthday { get; set; }

    [Key(5)]
    public required string LicenseNumber { get; set; }

    [Key(6)]
    public LicenseType LicenseType { get; set; }

    [Key(7)]
    public required string LicenseImage { get; set; }
}

[MessagePackObject]
public record CreateUserSuccessEvent : CreateUserEvent;