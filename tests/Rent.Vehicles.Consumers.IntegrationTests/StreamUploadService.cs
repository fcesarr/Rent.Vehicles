


using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Rent.Vehicles.Services;
using Rent.Vehicles.Services.Exceptions;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Settings;

namespace Rent.Vehicles.Consumers.IntegrationTests;

public interface IStreamUploadService : IUploadService
{
    byte[] Bytes { get; set; }
}

public class StreamUploadService : UploadService, IStreamUploadService
{
    public StreamUploadService(ILogger<StreamUploadService> logger,
        IOptions<StreamUploadSetting> streamUploadSetting) : base(logger, streamUploadSetting)
    {
    }

    private byte[] _bytes = Array.Empty<byte>();

    public byte[] Bytes { get => _bytes; set => _bytes = value; }

    protected override Task<string> ToUploadAsync(string fileName,
        byte[] bytes,
        CancellationToken cancellationToken = default)
    {
        Bytes = bytes;

        return Task.FromResult(fileName);
    }
}
