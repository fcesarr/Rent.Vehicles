using MessagePack;

namespace Rent.Vehicles.Messages.Events;

[MessagePackObject]
public record UploadUserLicenseImageEvent : Messages.Event
{
    [Key(1)]
    public required string LicenseImage { get; init; }
}