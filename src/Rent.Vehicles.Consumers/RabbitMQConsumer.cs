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
        return Task.Run(() =>
        {
            lock (_model)
            {
                if(!_model.IsOpen)
                    return null;

                var basicGetResult = _model.BasicGet(_name, false);

                if (basicGetResult == null)
                {
                    return null;
                }

                return new ConsumerResponse { Id = basicGetResult.DeliveryTag, Data = basicGetResult.Body.ToArray() };
            }
        }, cancellationToken);
    }

    public Task AckAsync(dynamic id, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            _model.BasicAck(id, false);

            return Task.CompletedTask;
        }, cancellationToken);
    }

    public Task SubscribeAsync(string name, CancellationToken cancellationToken = default)
    {
        _name = name;
        
        return Task.Run(() => _model.QueueDeclare(name,
            true,
            false,
            false), cancellationToken);
    }

    public Task RemoveAsync(dynamic id, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            _model.BasicReject(id, false);

            return Task.CompletedTask;
        }, cancellationToken);
    }

    public Task NackAsync(dynamic id, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            _model.BasicNack(id, false, true);

            return Task.CompletedTask;
        }, cancellationToken);
    }
}
