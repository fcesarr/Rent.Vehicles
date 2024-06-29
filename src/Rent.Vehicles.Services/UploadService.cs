using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Rent.Vehicles.Services.Exceptions;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Settings;
using Rent.Vehicles.Lib.Extensions;

namespace Rent.Vehicles.Services;

public abstract class UploadService : IUploadService
{
    private readonly UploadSetting _uploadSetting;
    private readonly ILogger<UploadService> _logger;

    public UploadService(ILogger<UploadService> logger,
        IOptions<UploadSetting> uploadSetting)
    {
        _logger = logger;
        _uploadSetting = uploadSetting.Value;
    }

    public Task<Result<string>> GetNameAsync(string base64String, CancellationToken cancellationToken = default)
    {
        return Task.Run<Result<string>>(() => {
            //
            var bytes = Convert.FromBase64String(base64String);

            var extension = GetExtension(bytes);

            if (extension == null)
            {
                return new NullException("Extensão não suportada.");
            }

            return $"{bytes.ByteToMD5String()}.{extension}";
            //
        }, cancellationToken);
    }

    public virtual async Task<Result<string>> UploadAsync(string base64String,
        CancellationToken cancellationToken = default)
    {
        var result = await GetNameAsync(base64String, cancellationToken);

        if(!result.IsSuccess)
            return result.Exception!;

        var bytes = Convert.FromBase64String(base64String);

        return await ToUploadAsync(result.Value!, bytes, cancellationToken);
    }

    protected abstract Task<string> ToUploadAsync(string name,
        byte[] bytes,
        CancellationToken cancellationToken = default);

    private string? GetExtension(byte[] bytes)
    {
        foreach (var signature in _uploadSetting.Formats)
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
