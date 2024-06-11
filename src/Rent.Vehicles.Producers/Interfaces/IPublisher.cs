using Rent.Vehicles.Messages;

namespace Rent.Vehicles.Producers.Interfaces;

public interface IPublisher
{
    Task PublishCommandAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : Message;

    Task PublishEventAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : Message;
}