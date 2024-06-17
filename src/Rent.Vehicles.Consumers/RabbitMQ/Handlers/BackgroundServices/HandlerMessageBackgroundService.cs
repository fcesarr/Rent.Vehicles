using System.Security.Cryptography;
using System.Text;

using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Exceptions;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages;

namespace Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;

public abstract class HandlerMessageBackgroundService<TEventToConsume> : BackgroundService 
    where TEventToConsume : Message
{
    protected readonly ILogger<HandlerMessageBackgroundService<TEventToConsume>> _logger;

    protected readonly IModel _channel;

    private readonly IPeriodicTimer _periodicTimer;

    protected readonly ISerializer _serializer;

    private readonly IDictionary<string, int> _retry = new Dictionary<string, int>();

    protected HandlerMessageBackgroundService(ILogger<HandlerMessageBackgroundService<TEventToConsume>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer)
    {
        _logger = logger;
        _channel = channel;
        _periodicTimer = periodicTimer;
        _serializer = serializer;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _channel.QueueDeclare(queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        return base.StartAsync(cancellationToken);
    }

    protected string QueueName { get; set; } = typeof(TEventToConsume).Name;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while(await _periodicTimer.WaitForNextTickAsync(cancellationToken))
        {
            BasicGetResult? result = default;

            try
            {
                result = _channel.BasicGet(QueueName, true);

                if(result == null)
                    continue;

                var bytes = result.Body.ToArray();

                var message = await _serializer.DeserializeAsync<TEventToConsume>(bytes, cancellationToken);

                if(message != null)
                {
                    await HandlerAsync(message, cancellationToken);
                }
            }
            catch(NoRetryException ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            catch (Exception ex)
            {   
                if(result != null)
                {
                    var hash = ComputeSha256Hash(result.Body.ToArray());
                    var count = 0;
                    if(_retry.TryGetValue(hash, out count))
                    {
                        _retry[hash] = ++count;
                    }

                    _ = _retry.TryAdd(hash, 0);

                    if(count < 3)
                        _channel.BasicNack(result.DeliveryTag, false, true);
                    else
                        _retry.Remove(hash);
                }
                
                _logger.LogError(ex, ex.Message);

            }
        }
    }
    private string ComputeSha256Hash(byte[] inputBytes)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            // Compute the SHA-256 hash
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            // Convert hash byte array to a hex string
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
    

    protected abstract Task HandlerAsync(TEventToConsume message, CancellationToken cancellationToken = default);
}