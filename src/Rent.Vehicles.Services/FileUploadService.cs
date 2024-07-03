using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Rent.Vehicles.Services.Exceptions;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Settings;
using Rent.Vehicles.Lib.Extensions;

namespace Rent.Vehicles.Services;

public class FileUploadService : UploadService, IUploadService
{
    private readonly Func<string, byte[], CancellationToken, Task> _func;
    private readonly FileUploadSetting _fileUploadSetting;

    public FileUploadService(ILogger<FileUploadService> logger,
        Func<string, byte[], CancellationToken, Task> func,
        IOptions<FileUploadSetting> fileUploadSetting) : base(logger, fileUploadSetting)
    {
        _func = func;
        _fileUploadSetting = fileUploadSetting.Value;
    }

    public override async Task<Result<string>> GetPathAsync(string base64String, CancellationToken cancellationToken = default)
    {
        var path = await base.GetPathAsync(base64String, cancellationToken);

        if(!path.IsSuccess)
            return path.Exception!;

        return $"{_fileUploadSetting.Path}/{path.Value}";
    }

    protected async override Task<string> ToUploadAsync(string path,
        byte[] bytes,
        CancellationToken cancellationToken = default)
    {
        if(!Directory.Exists(_fileUploadSetting.Path))
            Directory.CreateDirectory(_fileUploadSetting.Path);

        await _func(path, bytes, cancellationToken);

        return path;
    }
}
