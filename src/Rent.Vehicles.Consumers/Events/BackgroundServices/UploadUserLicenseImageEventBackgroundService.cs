using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Consumers.Events.BackgroundServices;

public class UploadUserLicenseImageEventBackgroundService : HandlerEventServicePublishBackgroundService<
    UploadUserLicenseImageEvent>
{
    public UploadUserLicenseImageEventBackgroundService(ILogger<UploadUserLicenseImageEventBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher,
        serviceScopeFactory)
    {
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(UploadUserLicenseImageEvent @event,
        CancellationToken cancellationToken = default)
    {
        var service = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<ILicenseImageService>();

        var result = await service.UploadAsync(@event.LicenseImage, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Exception!;
        }

        return result.Value!;
    }
}
