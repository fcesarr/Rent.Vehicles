
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices.Abstracts;

public abstract class DeleteBackgroundService<TMessage, TEntity> : HandlerCommandBackgroundService<TMessage, TEntity>
    where TMessage : Messages.Command
    where TEntity : Entities.Command
{
    protected readonly IDeleteService<TEntity> _deleteService;

    protected DeleteBackgroundService(ILogger<DeleteBackgroundService<TMessage, TEntity>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IDeleteService<TEntity> deleteService) : base(logger, channel, periodicTimer, serializer)
    {
        _deleteService = deleteService;
    }
}
