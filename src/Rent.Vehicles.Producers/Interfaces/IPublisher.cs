using Rent.Vehicles.Messages;

namespace Rent.Vehicles.Producers.Interfaces;

public interface IPublisher
{
    Task PublishCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : Command;

    Task PublishEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : Event;

    Task PublishSingleEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : Event;
}