using Amazon.Runtime.Internal.Util;

using Amqp;

using Microsoft.Extensions.Logging;

using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Lib.Responses;

namespace Rent.Vehicles.Lib;

public class AmqpConsumer : IConsumer
{
    private readonly ILogger<AmqpConsumer> _logger;
    private readonly Amqp.ISession _session;

    private IReceiverLink? _receiverLink;

    public AmqpConsumer(ILogger<AmqpConsumer> logger, Amqp.ISession session)
    {
        _logger = logger;
        _session = session;
    }

    public Task AckAsync(dynamic id, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => {
            if(_receiverLink is null)
                return;

            _receiverLink.Accept((Amqp.Message)id);
        }, cancellationToken);
    }

    public async Task<ConsumerResponse?> ConsumeAsync(CancellationToken cancellationToken = default)
    {
        if(_receiverLink is null)
            return null;

        var receivedMessage = await _receiverLink.ReceiveAsync();

        if(receivedMessage == null)
            return null;

        byte[]? receivedBytes = (byte[])receivedMessage.Body ?? Array.Empty<byte>();

        if(receivedBytes == null)
            return null;

        return new ConsumerResponse { Id = receivedMessage, Data = receivedBytes };
    }

    public Task RemoveAsync(dynamic id, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => {
            if(_receiverLink is null)
                return;
            
            _receiverLink?.Reject((Amqp.Message)id);
        }, cancellationToken);
    }

    public Task SubscribeAsync(string name, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => {
            _receiverLink = _session.CreateReceiver(Guid.NewGuid().ToString(), name);
        }, cancellationToken);
    }
}
