using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;

using LanguageExt;
using LanguageExt.Common;

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
            BasicGetResult? basicGetResult = default;

            try
            {
                basicGetResult = _channel.BasicGet(QueueName, false);

                if(basicGetResult == null)
                    continue;

                var bytes = basicGetResult.Body.ToArray();

                var message = await _serializer.DeserializeAsync<TEventToConsume>(bytes, cancellationToken);

                if(message == null)
                    continue;
                
                var result = await HandlerAsync(message, cancellationToken);

                await result.Match(async result => {
                        await result;

                        _channel.BasicAck(basicGetResult.DeliveryTag, true);
                    }, exception => exception switch
                    {
                        NoRetryException => TreatNoRetryException(exception),
                        _ => TreatException(basicGetResult, _retry, _channel, exception) 
                    });    
            }
            catch (Exception ex)
            {   
                _logger.LogError(ex, ex.Message);
            }
        }
    }

    private Task TreatNoRetryException(Exception exception)
    {
        _logger.LogError(exception, exception.Message);

        return Task.CompletedTask;
    }

    private Task TreatException(BasicGetResult basicGetResult,
        IDictionary<string, int> retry,
        IModel channel, 
        Exception exception)
    {
        if(basicGetResult != null)
        {
            var hash = ComputeSha256Hash(basicGetResult.Body.ToArray());

            if (retry.TryGetValue(hash, out int count))
            {
                retry[hash] = ++count;
            }

            _ = retry.TryAdd(hash, 0);

            if(count == 3)
            {
                channel.BasicReject(basicGetResult.DeliveryTag, false);
            }
        }
        
        _logger.LogError(exception, exception.Message);

        return Task.CompletedTask;
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
    

    protected abstract Task<Result<Task>> HandlerAsync(TEventToConsume message, CancellationToken cancellationToken = default);
}