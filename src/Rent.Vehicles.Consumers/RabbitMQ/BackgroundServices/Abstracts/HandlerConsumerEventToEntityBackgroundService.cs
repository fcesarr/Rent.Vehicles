using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices.Abstracts;

public abstract class HandlerConsumerEventToEntityBackgroundService<TEvent, TEntity> : BackgroundService 
    where TEvent : Messages.Event
    where TEntity : Entities.Entity
{
    protected readonly ILogger<HandlerConsumerEventToEntityBackgroundService<TEvent, TEntity>> _logger;

    private readonly IModel _channel;

    private readonly IPeriodicTimer _periodicTimer;

    private readonly ISerializer _serializer;

    protected HandlerConsumerEventToEntityBackgroundService(ILogger<HandlerConsumerEventToEntityBackgroundService<TEvent, TEntity>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer)
    {
        _logger = logger;
        _channel = channel;
        _periodicTimer = periodicTimer;
        _serializer = serializer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(await _periodicTimer.WaitForNextTickAsync(stoppingToken))
        {
            BasicGetResult? result = default;

            try
            {
                result = _channel.BasicGet(typeof(TEvent).Name, true);

                if(result == null)
                    continue;

                var bytes = result.Body.ToArray();

                var message = await _serializer.DeserializeAsync<TEvent>(bytes, stoppingToken);

                if(message != null)
                {
                    var entity = await EventToEntityAsync(message, stoppingToken);

                    await HandlerAsync(entity, stoppingToken);
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

    protected abstract Task HandlerAsync(TEntity entity, CancellationToken cancellationToken = default);

    protected abstract Task<TEntity> EventToEntityAsync(TEvent message, CancellationToken cancellationToken = default);
}