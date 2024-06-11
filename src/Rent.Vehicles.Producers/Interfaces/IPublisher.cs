using Rent.Vehicles.Messages;

namespace Rent.Vehicles.Producers.Interfaces;

public interface IPublisher
{
    Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : Message;
}