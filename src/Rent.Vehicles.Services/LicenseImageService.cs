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
            return new Result<Task>(new NullException());

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
                return new Result<string>(new NullException());

            var fileName = GetMd5Hash(fileBytes);

            var filePath = $"{_licenseImageServiceSetting.Path}/{fileName}.{fileExtension}";

            return filePath;
        }, cancellationToken);
    }

    private  string GetMd5Hash(byte[] input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] hashBytes = md5.ComputeHash(input);

            // Convert byte array to a hexadecimal string
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}