using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Consumers.Mappings;

namespace Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices;

public sealed class CreateBackgroundService<T, H> : BackgroundService where T : Message where H : Entity
{
    private readonly ILogger<CreateBackgroundService<T, H>> _logger;

    private readonly IModel _channel;

    private readonly IPeriodicTimer _periodicTimer;

    private readonly ISerializer _serializer;

    private readonly ICreateService<H> _createService;

    public CreateBackgroundService(ILogger<CreateBackgroundService<T, H>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        ICreateService<H> createService)
    {
        _logger = logger;
        _channel = channel;
        _periodicTimer = periodicTimer;
        _serializer = serializer;
        _createService = createService;
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
                        .MapCreateVehiclesCommandToCommand<H>(_serializer);

                    await _createService.Create(entity, stoppingToken);    
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
