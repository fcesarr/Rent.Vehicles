using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices.Abstracts;

public abstract class HandlerConsumerEventBackgroundService<TEvent, TEntity> : BackgroundService 
    where TEvent : Messages.Event
    where TEntity : Entities.Entity
{
    protected readonly ILogger<HandlerConsumerEventBackgroundService<TEvent, TEntity>> _logger;

    private readonly IModel _channel;

    private readonly IPeriodicTimer _periodicTimer;

    private readonly ISerializer _serializer;

    private readonly string _queueName;

    protected HandlerConsumerEventBackgroundService(ILogger<HandlerConsumerEventBackgroundService<TEvent, TEntity>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        string queueName)
    {
        _logger = logger;
        _channel = channel;
        _periodicTimer = periodicTimer;
        _serializer = serializer;
        _queueName = queueName;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(await _periodicTimer.WaitForNextTickAsync(stoppingToken))
        {
            BasicGetResult? result = default;

            try
            {
                result = _channel.BasicGet(_queueName, true);

                if(result == null)
                    continue;

                var bytes = result.Body.ToArray();

                var message = await _serializer.DeserializeAsync<TEvent>(bytes, stoppingToken);

                if(message != null)
                {
                    await HandlerAsync(message, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                if(result != null)
                    _channel.BasicNack(result.DeliveryTag, false, true);
                
                _logger.LogError(ex.Message, ex);
            }
        }
    }

    protected abstract Task HandlerAsync(TEvent @event, CancellationToken cancellationToken = default);
}