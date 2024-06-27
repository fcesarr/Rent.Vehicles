using System.ComponentModel.DataAnnotations;

using MessagePack;

namespace Rent.Vehicles.Messages.Events;

[MessagePackObject]
public record UpdateUserLicenseImageEvent : Messages.Event
{
    [MessagePack.Key(1)]
    public required Guid Id
    {
        get;
        init;
    }

    [MessagePack.Key(2)]
    [Base64String]
    public required string LicenseImage
    {
        get;
        init;
    }
}
