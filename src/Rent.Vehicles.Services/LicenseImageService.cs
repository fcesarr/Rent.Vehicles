using Microsoft.Extensions.Options;

using Rent.Vehicles.Lib.Extensions;
using Rent.Vehicles.Services.Exceptions;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Settings;

namespace Rent.Vehicles.Services;

public class LicenseImageService : ILicenseImageService
{
    private readonly LicenseImageSetting _licenseImageServiceSetting;

    private readonly IUploadService _uploadService;

    public LicenseImageService(IOptions<LicenseImageSetting> licenseImageServiceSetting,
        IUploadService uploadService)
    {
        _licenseImageServiceSetting = licenseImageServiceSetting.Value;
        _uploadService = uploadService;
    }

    public async Task<Result<Task>> UploadAsync(string licenseImage, CancellationToken cancellationToken = default)
    {
        var fileBytes = Convert.FromBase64String(licenseImage);

        var filePath = await GetPathAsync(licenseImage, cancellationToken);

        if (!filePath.IsSuccess)
        {
            return filePath.Exception!;
        }

        await _uploadService.UploadAsync(filePath.Value!, fileBytes, cancellationToken);

        return Task.CompletedTask;
    }

    public Task<Result<string>> GetPathAsync(string licenseImage, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            var fileBytes = Convert.FromBase64String(licenseImage);

            var fileExtension = GetFileExtension(fileBytes);

            if (fileExtension == null)
            {
                return Result<string>.Failure(new NullException("Extensão não suportada."));
            }

            var fileName = fileBytes.ByteToMD5String();

            var filePath = $"{_licenseImageServiceSetting.Path}/{fileName}.{fileExtension}";

            return filePath;
        }, cancellationToken);
    }

    private string? GetFileExtension(byte[] bytes)
    {
        foreach (var signature in _licenseImageServiceSetting.Formats)
        {
            var sigBytes = signature.Value;
            if (bytes.Length >= sigBytes.Length)
            {
                var isMatch = true;
                for (var i = 0; i < sigBytes.Length; i++)
                {
                    if (bytes[i] != sigBytes[i])
                    {
                        isMatch = false;
                        break;
                    }
                }

                if (isMatch)
                {
                    return signature.Key;
                }
            }
        }

        return null;
    }
}
