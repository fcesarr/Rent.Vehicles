using System.ComponentModel.DataAnnotations;

namespace Rent.Vehicles.Services.Settings;

public class UploadSetting
{
    [Required]
    public required Dictionary<string, byte[]> Formats
    {
        get;
        set;
    }
}
