using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using LanguageExt.Common;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class CreateUserSuccessEventUploadLicenseImageBackgroundService : HandlerEventServicePublishBackgroundService<
    CreateUserSuccessEvent,
    Task,
    ILicenseImageService>
{
    public CreateUserSuccessEventUploadLicenseImageBackgroundService(ILogger<CreateUserSuccessEventUploadLicenseImageBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        ILicenseImageService service) : base(logger, channel, periodicTimer, serializer, publisher, service)
    {
        QueueName = $"{typeof(CreateUserSuccessEvent).Name}.UploadLicenseImage";
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var result = base.StartAsync(cancellationToken);

        _channel.QueueBind(queue: QueueName,
            exchange: typeof(CreateUserSuccessEvent).Name, routingKey:"");
        
        return result;
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(CreateUserSuccessEvent command, CancellationToken cancellationToken = default)
    {
        var result = new Result<Task>(Task.CompletedTask);
        
        await _service.UploadAsync(command.LicenseImage, cancellationToken);

        return Task.FromResult(result);
    }
}
