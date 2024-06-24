
using Rent.Vehicles.Consumers.Responses;

namespace Rent.Vehicles.Consumers.Interfaces;

public interface IConsumer
{
    Task SubscribeAsync(string name, CancellationToken cancellationToken = default);

    Task<ConsumerResponse?> ConsumeAsync(CancellationToken cancellationToken = default);

    Task NackAsync(dynamic id, CancellationToken cancellationToken = default);
}


