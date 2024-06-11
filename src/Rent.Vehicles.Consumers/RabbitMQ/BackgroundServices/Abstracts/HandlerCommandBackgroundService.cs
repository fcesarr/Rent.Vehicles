using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Producers.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices.Abstracts;

public abstract class HandlerCommandBackgroundService <TCommand, TEvent, TEntity> : BackgroundService 
    where TCommand : Messages.Command
    where TEvent : Messages.Event
    where TEntity : Entities.Command
{
    private readonly ILogger<HandlerCommandBackgroundService<TCommand, TEvent, TEntity>> _logger;

    private readonly IModel _channel;

    private readonly IPeriodicTimer _periodicTimer;

    private readonly ISerializer _serializer;

    private readonly IPublisher _publisher;

    public HandlerCommandBackgroundService(ILogger<HandlerCommandBackgroundService<TCommand, TEvent, TEntity>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher)
    {
        _logger = logger;
        _channel = channel;
        _periodicTimer = periodicTimer;
        _serializer = serializer;
        _publisher = publisher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(await _periodicTimer.WaitForNextTickAsync(stoppingToken))
        {
            BasicGetResult? result = default;

            try
            {
                result = _channel.BasicGet(typeof(TCommand).Name, true);

                if(result == null)
                    continue;

                var bytes = result.Body.ToArray();

                var message = await _serializer.DeserializeAsync<TCommand>(bytes, stoppingToken);

                if(message != null)
                {
                    var entity = await CommandToEntityAsync(message, _serializer, stoppingToken);

                    await HandlerAsync(entity, stoppingToken);

                    var @event = await CommandToEventAsync(message, stoppingToken);

                    await _publisher.PublishAsync(@event, stoppingToken);
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

    protected abstract Task<TEntity> CommandToEntityAsync(TCommand message, ISerializer serializer, CancellationToken cancellationToken = default);

    protected abstract Task<TEvent> CommandToEventAsync(TCommand message, CancellationToken cancellationToken = default);
}