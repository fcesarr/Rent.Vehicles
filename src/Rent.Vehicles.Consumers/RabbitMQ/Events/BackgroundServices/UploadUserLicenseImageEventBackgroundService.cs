using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using LanguageExt.Common;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class UploadUserLicenseImageEventBackgroundService : HandlerEventServicePublishBackgroundService<
    UploadUserLicenseImageEvent,
    ILicenseImageService>
{
    public UploadUserLicenseImageEventBackgroundService(ILogger<UploadUserLicenseImageEventBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        ILicenseImageService service) : base(logger, channel, periodicTimer, serializer, publisher, service)
    {
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(UploadUserLicenseImageEvent @event, CancellationToken cancellationToken = default)
    {
        return await _service.UploadAsync(@event.LicenseImage, cancellationToken);
    }
}
