namespace Rent.Vehicles.Consumers.Utils.Interfaces;

public interface IPeriodicTimer
{
    ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default);
}