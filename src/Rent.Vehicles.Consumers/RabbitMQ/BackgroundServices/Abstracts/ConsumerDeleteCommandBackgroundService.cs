
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices.Abstracts;

public abstract class ConsumerDeleteCommandBackgroundService<TCommand, TEvent, TEntity> : HandlerConsumerCommandToEntityBackgroundService<TCommand, TEvent, TEntity>
    where TCommand : Messages.Command
    where TEvent : Messages.Event
    where TEntity : Entities.Command
{
    protected readonly IDeleteService<TEntity> _deleteService;

    protected ConsumerDeleteCommandBackgroundService(ILogger<ConsumerDeleteCommandBackgroundService<TCommand, TEvent, TEntity>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IDeleteService<TEntity> deleteService) : base(logger, channel, periodicTimer, serializer, publisher)
    {
        _deleteService = deleteService;
    }
}
