using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;

using LanguageExt;
using LanguageExt.Common;

using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Exceptions;
using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Responses;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages;

namespace Rent.Vehicles.Consumers.Handlers.BackgroundServices;

public abstract class HandlerMessageBackgroundService<TEventToConsume> : BackgroundService 
    where TEventToConsume : Message
{
    protected readonly ILogger<HandlerMessageBackgroundService<TEventToConsume>> _logger;

    protected readonly IConsumer _channel;

    private readonly IPeriodicTimer _periodicTimer;

    protected readonly ISerializer _serializer;

    private readonly IDictionary<string, int> _retry = new Dictionary<string, int>();

    protected HandlerMessageBackgroundService(ILogger<HandlerMessageBackgroundService<TEventToConsume>> logger,
        IConsumer channel,
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
        _channel.SubscribeAsync(QueueName, cancellationToken);

        return base.StartAsync(cancellationToken);
    }

    protected string QueueName { get; set; } = typeof(TEventToConsume).Name;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while(await _periodicTimer.WaitForNextTickAsync(cancellationToken))
        {
            ConsumerResponse? consumerResponse = default;

            try
            {
                consumerResponse = await _channel.ConsumeAsync(cancellationToken);

                if(consumerResponse == null)
                    continue;

                var bytes = consumerResponse.Data;

                var message = await _serializer.DeserializeAsync<TEventToConsume>(bytes, cancellationToken);

                if(message == null)
                    continue;
                
                var result = await HandlerAsync(message, cancellationToken);

                await result.Match(async result => {
                        await result;

                    }, exception => exception switch
                    {
                        NoRetryException => TreatNoRetryExceptionAsync(exception),
                        RetryException => TreatRetryExceptionAsync(consumerResponse,
                            exception,
                            cancellationToken),
                        _ => throw exception
                    });
            }
            catch (Exception ex)
            {   
                _logger.LogError(ex, ex.Message);
            }
        }
    }

    private Task TreatNoRetryExceptionAsync(Exception exception)
    {
        _logger.LogError(exception, exception.Message);

        return Task.CompletedTask;
    }

    private Task TreatRetryExceptionAsync(ConsumerResponse consumerResponse,
        Exception exception, 
        CancellationToken cancellationToken = default)
    {
        if(consumerResponse != null)
        {
            var hash = ComputeSha256Hash(consumerResponse.Data);

            if (_retry.TryGetValue(hash, out int count))
            {
                _retry[hash] = ++count;
            }

            _ = _retry.TryAdd(hash, 0);

            if(count > 3)
            {
                _channel.NackAsync(consumerResponse.Id, cancellationToken);
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
    

    protected virtual async Task<Result<Task>> HandlerAsync(TEventToConsume message, CancellationToken cancellationToken = default)
        => await HandlerMessageAsync(message, cancellationToken);
    
    protected abstract Task<Result<Task>> HandlerMessageAsync(TEventToConsume message, CancellationToken cancellationToken = default);
}