using Microsoft.Extensions.Logging;

using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Services;

public class FileUploadService : IUploadService
{
    private readonly Func<string, byte[], CancellationToken, Task> _func;
    private readonly ILogger<FileUploadService> _logger;

    public FileUploadService(ILogger<FileUploadService> logger,
        Func<string, byte[], CancellationToken, Task> func)
    {
        _logger = logger;
        _func = func;
    }

    public async Task<Result<Task>> UploadAsync(string filePath, byte[] fileBytes,
        CancellationToken cancellationToken = default)
    {
        await _func(filePath, fileBytes, cancellationToken);

        return Task.CompletedTask;
    }
}
