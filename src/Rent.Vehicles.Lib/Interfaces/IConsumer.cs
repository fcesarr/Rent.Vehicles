using Rent.Vehicles.Lib.Responses;

namespace Rent.Vehicles.Lib.Interfaces;

public interface IConsumer
{
    Task SubscribeAsync(string name, CancellationToken cancellationToken = default);

    Task<ConsumerResponse?> ConsumeAsync(CancellationToken cancellationToken = default);

    Task AckAsync(dynamic id, CancellationToken cancellationToken = default);

    Task RemoveAsync(dynamic id, CancellationToken cancellationToken = default);
}
