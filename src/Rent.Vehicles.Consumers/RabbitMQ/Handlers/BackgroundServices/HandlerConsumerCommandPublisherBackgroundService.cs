
using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;

public abstract class HandlerConsumerCommandPublisherBackgroundService<TCommand, TEvent> : HandlerConsumerMessageBackgroundService<TCommand> 
    where TCommand : Messages.Command
    where TEvent : Messages.Event
{
    protected readonly IPublisher _publisher;

    protected readonly ICreateService<Command> _createService;

    protected HandlerConsumerCommandPublisherBackgroundService(ILogger<HandlerConsumerCommandPublisherBackgroundService<TCommand, TEvent>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        ICreateService<Command> createService) : base(logger, channel, periodicTimer, serializer)
    {
        _publisher = publisher;
        _createService = createService;
    }

    protected abstract Task<Command> CommandToEntityAsync(TCommand message, ISerializer serializer, CancellationToken cancellationToken = default);

    protected abstract Task<TEvent> CommandToEventAsync(TCommand message, CancellationToken cancellationToken = default);

    protected override async Task HandlerAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var entity = await CommandToEntityAsync(command, _serializer, cancellationToken);

        await HandlerAsync(entity, cancellationToken);

        var @event = await CommandToEventAsync(command, cancellationToken);

        await _publisher.PublishEventAsync(@event, cancellationToken);
    }

    private async Task HandlerAsync(Command entity, CancellationToken cancellationToken = default)
    {
        await _createService.CreateAsync(entity, cancellationToken);
    }
}