using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Services;

namespace Rent.Vehicles.Consumers.Events.BackgroundServices;

public class UploadUserLicenseImageEventBackgroundService : HandlerEventServicePublishBackgroundService<
    UploadUserLicenseImageEvent,
    ILicenseImageService>
{
    public UploadUserLicenseImageEventBackgroundService(ILogger<UploadUserLicenseImageEventBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        ILicenseImageService service) : base(logger, channel, periodicTimer, serializer, publisher, service)
    {
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(UploadUserLicenseImageEvent @event, CancellationToken cancellationToken = default)
    {
        var result = await _service.UploadAsync(@event.LicenseImage, cancellationToken);

        if(!result.IsSuccess)
            return result.Exception!;

        return result.Value!;
    }
}
