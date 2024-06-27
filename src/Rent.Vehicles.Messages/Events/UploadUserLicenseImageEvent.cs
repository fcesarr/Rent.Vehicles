using MessagePack;

namespace Rent.Vehicles.Messages.Events;

[MessagePackObject]
public record UploadUserLicenseImageEvent : Messages.Event
{
    [Key(1)]
    public required Guid Id
    {
        get;
        init;
    }

    [Key(2)]
    public required string LicenseImage
    {
        get;
        init;
    }
}
