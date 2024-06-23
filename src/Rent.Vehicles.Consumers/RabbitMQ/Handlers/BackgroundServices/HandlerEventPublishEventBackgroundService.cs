using LanguageExt;
using LanguageExt.Common;
using LanguageExt.TypeClasses;

using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Producers.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;

public abstract class HandlerEventPublishEventBackgroundService<TEventToConsume> : HandlerEventPublishBackgroundService<TEventToConsume>
    where TEventToConsume : Messages.Event
{
    protected HandlerEventPublishEventBackgroundService(ILogger<HandlerEventPublishEventBackgroundService<TEventToConsume>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher) : base(logger, channel, periodicTimer, serializer, publisher)
    {
    }

    protected override async Task<Result<Task>> HandlerAsync(TEventToConsume @event, CancellationToken cancellationToken = default)
    {
        var result = await base.HandlerAsync(@event, cancellationToken);

        return result.Match(result => {
            result.GetAwaiter().GetResult();

            var eventToPublish = CreateEventToPublish(@event);
        
            PublishAsync(eventToPublish, cancellationToken).GetAwaiter().GetResult();

            return Task.CompletedTask;
        }, exception => new Result<Task>(exception));
    }

 
    protected abstract IEnumerable<Messages.Event> CreateEventToPublish(TEventToConsume @event);

    protected virtual async Task PublishAsync(IEnumerable<Messages.Event> events, CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task>();
        
        foreach (var @event in events)
        {
            tasks.Add(_publisher.PublishSingleEventAsync(@event, cancellationToken));
        }

        await Task.WhenAll(tasks);
    }
}