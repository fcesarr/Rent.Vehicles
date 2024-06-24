
using RabbitMQ.Client;

using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Producers.Interfaces;

namespace Rent.Vehicles.Producers.RabbitMQ;

public class Publisher : IPublisher
{
    private readonly IModel _channel;

    private readonly ISerializer _serializer;

    public Publisher(IModel channel, ISerializer serializer)
    {
        _channel = channel;
        _serializer = serializer;
    }

    public async Task PublishCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : Command
    {
        _channel.BasicPublish(exchange: string.Empty,
            routingKey:  command.GetType().Name,
            basicProperties: null,
            body: await _serializer.SerializeAsync(command, cancellationToken));
    }

    public async Task PublishEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : Event
    {
        _channel.BasicPublish(exchange: @event.GetType().Name,
            routingKey: string.Empty,
            basicProperties: null,
            body: await _serializer.SerializeAsync(@event, cancellationToken));
    }

    public async Task PublishSingleEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : Event
    {
        _channel.BasicPublish(exchange: string.Empty,
            routingKey: @event.GetType().Name,
            basicProperties: null,
            body: await _serializer.SerializeAsync(@event, @event.GetType(), cancellationToken));
    }
}