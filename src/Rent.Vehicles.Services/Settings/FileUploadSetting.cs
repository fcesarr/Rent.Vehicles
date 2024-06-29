using System.ComponentModel.DataAnnotations;

namespace Rent.Vehicles.Services.Settings;

public class FileUploadSetting : UploadSetting
{
    [Required]
    public required string Path { get; set; }
}
