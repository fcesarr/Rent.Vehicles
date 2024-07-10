using Amqp;

using Microsoft.Extensions.Logging;

using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Lib.Responses;

namespace Rent.Vehicles.Lib;

public class AmqpConsumer : IConsumer
{
    private readonly ILogger<AmqpConsumer> _logger;
    private readonly ISession _session;

    private IReceiverLink? _receiverLink;

    public AmqpConsumer(ILogger<AmqpConsumer> logger, ISession session)
    {
        _logger = logger;
        _session = session;
    }

    public Task AckAsync(dynamic id, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            if (_receiverLink is null)
            {
                return;
            }

            _receiverLink.Accept((Amqp.Message)id);
        }, cancellationToken);
    }

    public async Task<ConsumerResponse?> ConsumeAsync(CancellationToken cancellationToken = default)
    {
        if (_receiverLink is null)
        {
            return null;
        }

        try
        {
            var receivedMessage = await _receiverLink.ReceiveAsync();
    
            if (receivedMessage == null)
            {
                return null;
            }

            var receivedBytes = receivedMessage.Body as byte[] ?? Array.Empty<byte>();

            if (receivedBytes.Length == 0)
            {
                // Rejeitar mensagem vazia ou inválida
                _receiverLink.Reject(receivedMessage);
                _logger.LogWarning("Received empty message from {QueueName}", _receiverLink.Name);
                return null;
            }

            return new ConsumerResponse { Id = receivedMessage, Data = receivedBytes };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on Consume message {QueueName}", _receiverLink.Name);
            return null;
        }
    }

    public Task RemoveAsync(dynamic id, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            if (_receiverLink is null)
            {
                return;
            }

            _receiverLink?.Reject((Amqp.Message)id);
        }, cancellationToken);
    }

    public Task SubscribeAsync(string name, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            _receiverLink = _session.CreateReceiver(Guid.NewGuid().ToString(), name);
        }, cancellationToken);
    }
}
