using Amqp;

using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Lib.Responses;

namespace Rent.Vehicles.Lib;

public class AmqpConsumer : IConsumer
{
    private readonly Amqp.ISession _session;

    private IReceiverLink? _receiverLink;

    public AmqpConsumer(Amqp.ISession session)
    {
        _session = session;
    }

    public Task AckAsync(dynamic id, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => {
            try
            {
                _receiverLink?.Accept((Amqp.Message)id);
            }
            catch (System.Exception ex)
            {
                
                return;
            }
        }, cancellationToken);
    }

    public async Task<ConsumerResponse?> ConsumeAsync(CancellationToken cancellationToken = default)
    {
        Amqp.Message? receivedMessage;
        
        try
        {
            receivedMessage = await _receiverLink?.ReceiveAsync();
    
            if(receivedMessage == null)
                return null;

            byte[]? receivedBytes = (byte[])receivedMessage.Body ?? Array.Empty<byte>();
    
            if(receivedBytes == null)
                return null;
    
            return new ConsumerResponse { Id = receivedMessage, Data = receivedBytes };
        }
        catch (System.Exception ex)
        {
            
            return null;
        }
    }

    public Task RemoveAsync(dynamic id, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => _receiverLink?.Reject((Amqp.Message)id), cancellationToken);
    }

    public Task SubscribeAsync(string name, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => {
            _receiverLink = new ReceiverLink((Session)_session, Guid.NewGuid().ToString(), name);
        }, cancellationToken);
    }
}
