using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices;

public sealed class CreateBackgroundService<T> : BackgroundService where T : Message
{
    private readonly ILogger<CreateBackgroundService<T>> _logger;

    private readonly IModel _channel;

    private readonly IPeriodicTimer _periodicTimer;

    private readonly ISerializer _serializer;

    private readonly ICreateService<T> _createService;

    public CreateBackgroundService(ILogger<CreateBackgroundService<T>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        ICreateService<T> createService)
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
                    await _createService.Create(message, stoppingToken);
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
