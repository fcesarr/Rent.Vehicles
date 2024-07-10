using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Rent.Vehicles.Services;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Consumers.IntegrationTests;

public interface IStreamUploadService : IUploadService
{
}

public class StreamUploadService : UploadService, IStreamUploadService
{
    public StreamUploadService(ILogger<StreamUploadService> logger,
        IOptions<StreamUploadSetting> streamUploadSetting) : base(logger, streamUploadSetting)
    {
    }

    protected override Task<string> ToUploadAsync(string name,
        byte[] bytes,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(name);
    }
}
