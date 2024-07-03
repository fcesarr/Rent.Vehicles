using MessagePack;

using Rent.Vehicles.Messages.Types;

namespace Rent.Vehicles.Messages.Events;

[MessagePackObject]
public record UpdateUserEvent : Messages.Event
{
    [Key(1)]
    public required Guid Id
    {
        get;
        init;
    }

    [Key(2)]
    public string? Name
    {
        get;
        set;
    }

    [Key(3)]
    public string? Number
    {
        get;
        set;
    }

    [Key(4)]
    public DateTime? Birthday
    {
        get;
        set;
    }

    [Key(5)]
    public string? LicenseNumber
    {
        get;
        set;
    }

    [Key(6)]
    public LicenseType? LicenseType
    {
        get;
        set;
    }
}
