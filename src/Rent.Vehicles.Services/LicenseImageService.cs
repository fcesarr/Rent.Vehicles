using System.Security.Cryptography;
using System.Text;

using LanguageExt.Common;

using Microsoft.Extensions.Options;

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

        var fileExtension = GetFileExtension(fileBytes);

        if (fileExtension == null)
            return Result<Task>.Failure(new NullException());

        var fileName = GetMd5Hash(fileBytes);

        var filePath = $"{_licenseImageServiceSetting.Path}/{fileName}.{fileExtension}";

        await _uploadService.UploadAsync(filePath, fileBytes, cancellationToken);
        
        return Task.CompletedTask;
    }   

    private string? GetFileExtension(byte[] bytes)
    {
        foreach (var signature in _licenseImageServiceSetting.Formats)
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

    public Task<Result<string>> GetPathAsync(string licenseImage, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => {
            byte[] fileBytes = Convert.FromBase64String(licenseImage);

            var fileExtension = GetFileExtension(fileBytes);

            if (fileExtension == null)
                return Result<string>.Failure(new NullException());

            var fileName = GetMd5Hash(fileBytes);

            var filePath = $"{_licenseImageServiceSetting.Path}/{fileName}.{fileExtension}";

            return filePath;
        }, cancellationToken);
    }

    private static string GetMd5Hash(byte[] input)
    {
        byte[] hashBytes = MD5.HashData(input);

        // Convert byte array to a hexadecimal string
        var stringBuilder = new StringBuilder();
        foreach (byte b in hashBytes)
        {
            stringBuilder.Append(b.ToString("x2"));
        }
        return stringBuilder.ToString();
    }
}