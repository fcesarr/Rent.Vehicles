
using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Responses;

namespace Rent.Vehicles.Consumers;

public class RabbitMQConsumer : IConsumer
{
    private readonly IModel _model;

    private string _name = string.Empty;

    public RabbitMQConsumer(IModel model)
    {
        _model = model;
    }

    public Task<ConsumerResponse?> ConsumeAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() => {
            var basicGetResult = _model.BasicGet(_name, true);

            if(basicGetResult == null)
                return null;

            return new ConsumerResponse
            {
                Id = basicGetResult.DeliveryTag,
                Data = basicGetResult.Body.ToArray()
            };
        }, cancellationToken);
    }

    public Task NackAsync(dynamic id, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => {
            _model.BasicNack(id, false, true);
        }, cancellationToken);
    }

    public Task SubscribeAsync(string name, CancellationToken cancellationToken = default)
    {
        _name = name;
        return Task.Run(() => _model.QueueDeclare(queue: name,
                durable: true,
                exclusive: false,
                autoDelete: false), cancellationToken);
    }
}
