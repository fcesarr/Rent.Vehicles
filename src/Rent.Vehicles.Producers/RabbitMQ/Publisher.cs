
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

    public async Task PublishCommandAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : Message
    {
        _channel.BasicPublish(exchange: string.Empty,
            routingKey: typeof(TMessage).Name,
            basicProperties: null,
            body: await _serializer.SerializeAsync(message, cancellationToken));
    }

    public async Task PublishEventAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : Message
    {
        _channel.BasicPublish(exchange: typeof(TMessage).Name,
            routingKey: string.Empty,
            basicProperties: null,
            body: await _serializer.SerializeAsync(message, cancellationToken));
    }
}