using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Consumers.Mappings;

namespace Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices;

public sealed class DeleteBackgroundService<T, H> : BackgroundService where T : Message where H : Entity
{
    private readonly ILogger<DeleteBackgroundService<T, H>> _logger;

    private readonly IModel _channel;

    private readonly IPeriodicTimer _periodicTimer;

    private readonly ISerializer _serializer;

    private readonly IDeleteService<H> _deleteService;

    public DeleteBackgroundService(ILogger<DeleteBackgroundService<T, H>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IDeleteService<H> deleteService)
    {
        _logger = logger;
        _channel = channel;
        _periodicTimer = periodicTimer;
        _serializer = serializer;
        _deleteService = deleteService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(await _periodicTimer.WaitForNextTickAsync(stoppingToken))
        {
            BasicGetResult? result = default;

            try
            {
                result = _channel.BasicGet(typeof(T).Name, true);

                if(result == null)
                    continue;

                var bytes = result.Body.ToArray();

                var message = await _serializer.DeserializeAsync<T>(bytes, stoppingToken);

                if(message != null)
                {
                    var entity = await message
                        .MapCommandToCommand<H>(_serializer);

                    await _deleteService.DeleteAsync(entity, stoppingToken);    
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
}
