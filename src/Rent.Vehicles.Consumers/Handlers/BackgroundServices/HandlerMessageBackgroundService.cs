using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Responses;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Extensions;
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
        _channel.SubscribeAsync(typeof(TEventToConsume).Name, cancellationToken);

        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (await _periodicTimer.WaitForNextTickAsync(cancellationToken))
        {
            try
            {
                var consumerResponse = await _channel.ConsumeAsync(cancellationToken);

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

                var result = await HandlerAsync(message, cancellationToken);

                if (!result.IsSuccess)
                {
                    await (result.Exception switch
                    {
                        RetryException => TreatRetryExceptionAsync(consumerResponse,
                            result.Exception,
                            cancellationToken),
                        NoRetryException or _ => TreatNoRetryExceptionAsync(result.Exception!)
                    });

                    continue;
                }

                await _channel.AckAsync(consumerResponse.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                await TreatNoRetryExceptionAsync(ex);
            }
        }
    }

    private Task TreatNoRetryExceptionAsync(Exception exception)
    {
        _logger.LogError(exception, exception.Message);

        return Task.CompletedTask;
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

        _logger.LogError(exception, exception.Message);

        if (count < 3)
        {
            await _channel.NackAsync(consumerResponse.Id, cancellationToken);
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
