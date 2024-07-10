using RabbitMQ.Client;

using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;

namespace Rent.Vehicles.Lib;

public class RabbitMQPublisher : IPublisher
{
    private readonly IModel _channel;

    private readonly ISerializer _serializer;

    public RabbitMQPublisher(IModel channel, ISerializer serializer)
    {
        _channel = channel;
        _serializer = serializer;
    }

    public async Task PublishCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : Command
    {
        _channel.BasicPublish(string.Empty,
            command.GetType().Name,
            null,
            await _serializer.SerializeAsync(command, cancellationToken));
    }

    public async Task PublishEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : Event
    {
        _channel.BasicPublish(@event.GetType().Name,
            string.Empty,
            null,
            await _serializer.SerializeAsync(@event, cancellationToken));
    }

    public async Task PublishSingleEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : Event
    {
        _channel.BasicPublish(string.Empty,
            @event.GetType().Name,
            null,
            await _serializer.SerializeAsync(@event, @event.GetType(), cancellationToken));
    }
}
