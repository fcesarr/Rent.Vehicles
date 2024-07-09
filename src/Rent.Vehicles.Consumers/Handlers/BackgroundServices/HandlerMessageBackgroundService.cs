
using Microsoft.Extensions.Options;

using Rent.Vehicles.Consumers.Settings;
using Rent.Vehicles.Consumers.Types;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib;
using Rent.Vehicles.Lib.Extensions;
using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Lib.Responses;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.Exceptions;

namespace Rent.Vehicles.Consumers.Handlers.BackgroundServices;

public abstract class HandlerMessageBackgroundService<TEventToConsume> : BackgroundService
    where TEventToConsume : Message
{
    protected readonly IConsumer _channel;
    
    protected readonly ILogger<HandlerMessageBackgroundService<TEventToConsume>> _logger;

    private readonly IPeriodicTimer _periodicTimer;

    private readonly IDictionary<string, int> _retry = new Dictionary<string, int>();

    protected readonly ISerializer _serializer;

    private readonly ConsumerSetting _consumerSetting;

    private readonly string _guid;

    private readonly SemaphoreSlim _semaphoreSlim;

    private readonly string _queueName = typeof(TEventToConsume).Name;
    
    protected abstract ConsumerType _type { get; }

    protected HandlerMessageBackgroundService(ILogger<HandlerMessageBackgroundService<TEventToConsume>> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IOptions<ConsumerSetting> consumerSetting)
    {
        _logger = logger;
        _channel = channel;
        _periodicTimer = periodicTimer;
        _serializer = serializer;
        _consumerSetting = consumerSetting.Value;
        _guid = Guid.NewGuid().ToString();
        _semaphoreSlim = new SemaphoreSlim(1, _consumerSetting.BufferSize);
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        if(_consumerSetting.Type != ConsumerType.Both &&
            _consumerSetting.Type != _type)
            return;
        
        if(!_consumerSetting.ToIncluded.Any() && _consumerSetting.ToExcluded.Contains(_queueName))
            return;
        
        if(!_consumerSetting.ToExcluded.Any() && _consumerSetting.ToIncluded.Any() && !_consumerSetting.ToIncluded.Contains(_queueName))
            return;

        await _channel.SubscribeAsync(_queueName, cancellationToken);

        _logger.LogInformation("StartAsync {ClassName}: {Guid}", this.GetType().Name, _guid);

        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("StopAsync {ClassName}: {Guid}", this.GetType().Name, _guid);
        
        //await _channel.CloseAsync();

        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {   
            await _semaphoreSlim.WaitAsync(cancellationToken);
            
            ConsumerResponse? consumerResponse = default;
            try
            {
                consumerResponse = await _channel.ConsumeAsync(cancellationToken);

                if (consumerResponse == null)
                {
                    continue;
                }

                var bytes = consumerResponse.Data;

                var message =
                    await _serializer.DeserializeAsync<TEventToConsume>(bytes, cancellationToken);

                if (message == null)
                {
                    continue;
                }

                _logger.LogInformation("Handler message from type {MessageToConsume}", _queueName);

                var result = await HandlerAsync(message, cancellationToken);

                if (!result.IsSuccess)
                {
                    await (result.Exception switch
                    {
                        RetryException => TreatRetryExceptionAsync(consumerResponse,
                            result.Exception,
                            cancellationToken),
                        NoRetryException or _ => TreatNoRetryExceptionAsync(consumerResponse,
                            result.Exception!,
                            cancellationToken)
                    });

                    continue;
                }

                await _channel.AckAsync(consumerResponse.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                await TreatNoRetryExceptionAsync(consumerResponse, ex, cancellationToken);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }

    private async Task TreatNoRetryExceptionAsync(ConsumerResponse? consumerResponse,
        Exception exception, CancellationToken cancellationToken = default)
    {
        if(consumerResponse is not null)
            await _channel.RemoveAsync(consumerResponse.Id, cancellationToken);

        _logger.LogError(exception, "{ClassName} encountered an error: {ErrorMessage}", this.GetType().Name, exception.Message);
    }

    private async Task TreatRetryExceptionAsync(ConsumerResponse consumerResponse,
        Exception exception,
        CancellationToken cancellationToken = default)
    {
        var hash = consumerResponse.Data.ByteToMD5String();

        if (_retry.TryGetValue(hash, out var count) || _retry.TryAdd(hash, 0))
        {
            _retry[hash] = ++count;
        }

        _logger.LogError(exception, "{ClassName} encountered an error: {ErrorMessage}", this.GetType().Name, exception.Message);

        if (count < 3)
        {
            return;
        }

        await _channel.RemoveAsync(consumerResponse.Id, cancellationToken);
    }

    protected virtual async Task<Result<Task>> HandlerAsync(TEventToConsume message,
        CancellationToken cancellationToken = default)
    {
        return await HandlerMessageAsync(message, cancellationToken);
    }

    protected abstract Task<Result<Task>> HandlerMessageAsync(TEventToConsume message,
        CancellationToken cancellationToken = default);
}
