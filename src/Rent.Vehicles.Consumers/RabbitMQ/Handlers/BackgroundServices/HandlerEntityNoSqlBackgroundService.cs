
using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;

public abstract class HandlerEntityNoSqlBackgroundService<TEventToConsume, TEntity> : HandlerMessageBackgroundService<TEventToConsume> 
    where TEventToConsume : Messages.Event
    where TEntity : Entity
{
    protected readonly INoSqlService<TEntity> _service;

    protected HandlerEntityNoSqlBackgroundService(ILogger<HandlerEntityNoSqlBackgroundService<TEventToConsume, TEntity>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        INoSqlService<TEntity> service) : base(logger, channel, periodicTimer, serializer)
    {
        _service = service;
    }
}