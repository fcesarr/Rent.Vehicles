

using RabbitMQ.Client;

using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Consumers.BackgroundServices;

public abstract class ConsumerCreateCommandBackgroundService<TCommand, TEvent, TEntity> : HandlerConsumerCommandToEntityBackgroundService<TCommand, TEvent, TEntity>
    where TCommand : Messages.Command
    where TEvent : Messages.Event
    where TEntity : Entities.Command
{
    protected readonly ICreateService<TEntity> _createService;

    protected ConsumerCreateCommandBackgroundService(ILogger<ConsumerCreateCommandBackgroundService<TCommand, TEvent, TEntity>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        string queueName,
        IPublisher publisher,
        ICreateService<TEntity> createService) : base(logger, channel, periodicTimer, serializer, queueName, publisher)
    {
        _createService = createService;
    }

    protected override async Task HandlerAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _createService.CreateAsync(entity, cancellationToken);
    }
}
