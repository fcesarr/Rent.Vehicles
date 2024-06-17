using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.Interfaces;

using Event = Rent.Vehicles.Messages.Events.Event;

namespace Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;

public abstract class HandlerEventServicePublishBackgroundService<TEventToConsume, TEntity, TService> : HandlerEventPublishBackgroundService<TEventToConsume>
    where TEventToConsume : Messages.Event
    where TEntity : Entity
    where TService : IService<TEntity>
{
    protected readonly TService _service;

    protected HandlerEventServicePublishBackgroundService(ILogger<HandlerEventServicePublishBackgroundService<TEventToConsume, TEntity, TService>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        TService service) : base(logger, channel, periodicTimer, serializer, publisher)
    {
        _service = service;
    }


}