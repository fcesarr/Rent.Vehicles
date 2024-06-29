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

    protected async override Task<string> ToUploadAsync(string name,
        byte[] bytes,
        CancellationToken cancellationToken = default)
    {
        if(!Directory.Exists(_fileUploadSetting.Path))
            Directory.CreateDirectory(_fileUploadSetting.Path);

        var path = $"{_fileUploadSetting.Path}/{name}";

        await _func(path, bytes, cancellationToken);

        return path;
    }
}
