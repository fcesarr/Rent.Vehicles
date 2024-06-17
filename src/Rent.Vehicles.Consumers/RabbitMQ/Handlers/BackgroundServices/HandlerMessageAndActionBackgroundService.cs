
using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;

public abstract class HandlerMessageAndActionBackgroundService<TMessageToConsume, TEntity, TService> : HandlerMessageBackgroundService<TMessageToConsume> 
    where TMessageToConsume : Messages.Message
    where TEntity : Entity
    where TService : IService<TEntity>
{
    protected readonly TService _service;

    protected HandlerMessageAndActionBackgroundService(ILogger<HandlerMessageAndActionBackgroundService<TMessageToConsume, TEntity, TService>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        TService service) : base(logger, channel, periodicTimer, serializer)
    {
        _service = service;
    }
}