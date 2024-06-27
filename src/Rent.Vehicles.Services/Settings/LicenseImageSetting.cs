namespace Rent.Vehicles.Services.Settings;

public class LicenseImageSetting
{
    public required string Path
    {
        get;
        set;
    }

    public required Dictionary<string, byte[]> Formats
    {
        get;
        set;
    }
}
