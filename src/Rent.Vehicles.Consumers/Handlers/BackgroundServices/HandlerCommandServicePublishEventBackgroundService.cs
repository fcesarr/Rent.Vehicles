using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.Interfaces;


namespace Rent.Vehicles.Consumers.Handlers.BackgroundServices;

public abstract class HandlerCommandServicePublishEventBackgroundService<TCommandToConsume, TEventToPublish, TService> : HandlerCommandPublishEventBackgroundService<TCommandToConsume, TEventToPublish>
    where TCommandToConsume : Messages.Command
    where TEventToPublish : Messages.Event
    where TService : class
{
    protected readonly TService _service;

    protected HandlerCommandServicePublishEventBackgroundService(ILogger<HandlerCommandServicePublishEventBackgroundService<TCommandToConsume, TEventToPublish, TService>> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        TService service) : base(logger, channel, periodicTimer, serializer, publisher)
    {
        _service = service;
    }
}