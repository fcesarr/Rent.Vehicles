

using LanguageExt;
using LanguageExt.Common;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Rent.Vehicles.Services.Exceptions;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Settings;

namespace Rent.Vehicles.Services;

public class FileUploadService : IUploadService
{
    private readonly ILogger<FileUploadService> _logger;

    private readonly Func<string, byte[], CancellationToken, Task> _func;

    public FileUploadService(ILogger<FileUploadService> logger,
        Func<string, byte[], CancellationToken, Task> func)
    {
        _logger = logger;
        _func = func;
    }

    public async Task<Result<Task>> UploadAsync(string filePath, byte[] fileBytes, CancellationToken cancellationToken = default)
    {
        await _func(filePath, fileBytes, cancellationToken);

        return Task.CompletedTask;
    }
}