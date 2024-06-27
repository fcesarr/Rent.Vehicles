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
        byte[] fileBytes = Convert.FromBase64String(licenseImage);

        Result<string> filePath = await GetPathAsync(licenseImage, cancellationToken);

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
            byte[] fileBytes = Convert.FromBase64String(licenseImage);

            string? fileExtension = GetFileExtension(fileBytes);

            if (fileExtension == null)
            {
                return Result<string>.Failure(new NullException("Extensão não suportada."));
            }

            string fileName = fileBytes.ByteToMD5String();

            string filePath = $"{_licenseImageServiceSetting.Path}/{fileName}.{fileExtension}";

            return filePath;
        }, cancellationToken);
    }

    private string? GetFileExtension(byte[] bytes)
    {
        foreach (KeyValuePair<string, byte[]> signature in _licenseImageServiceSetting.Formats)
        {
            byte[] sigBytes = signature.Value;
            if (bytes.Length >= sigBytes.Length)
            {
                bool isMatch = true;
                for (int i = 0; i < sigBytes.Length; i++)
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