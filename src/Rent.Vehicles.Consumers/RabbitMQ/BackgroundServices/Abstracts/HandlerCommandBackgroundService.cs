using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices.Abstracts;

public abstract class HandlerCommandBackgroundService <TMessage, TEntity> : BackgroundService 
    where TMessage : Messages.Command 
    where TEntity : Entities.Command
{
    private readonly ILogger<HandlerCommandBackgroundService<TMessage, TEntity>> _logger;

    private readonly IModel _channel;

    private readonly IPeriodicTimer _periodicTimer;

    private readonly ISerializer _serializer;

    public HandlerCommandBackgroundService(ILogger<HandlerCommandBackgroundService<TMessage, TEntity>> logger,
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
                result = _channel.BasicGet(typeof(TMessage).Name, true);

                if(result == null)
                    continue;

                var bytes = result.Body.ToArray();

                var message = await _serializer.DeserializeAsync<TMessage>(bytes, stoppingToken);

                if(message != null)
                {
                    var entity = await CommandToEntity(message, _serializer);

                    await Handler(entity, stoppingToken);
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

    protected abstract Task Handler(TEntity entity, CancellationToken cancellationToken = default);

    protected abstract Task<TEntity> CommandToEntity(TMessage message, ISerializer serializer);
}