
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices.Abstracts;

public abstract class CreateBackgroundService<TMessage, TEntity> : HandlerCommandBackgroundService<TMessage, TEntity>
    where TMessage : Messages.Command
    where TEntity : Entities.Command
{
    protected readonly ICreateService<TEntity> _createService;

    protected CreateBackgroundService(ILogger<CreateBackgroundService<TMessage, TEntity>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        ICreateService<TEntity> createService) : base(logger, channel, periodicTimer, serializer)
    {
        _createService = createService;
    }

    
}
