
using Rent.Vehicles.Consumers.Utils.Interfaces;

namespace Rent.Vehicles.Consumers.Utils;

public class PeriodicTimer : IPeriodicTimer
{
    private readonly System.Threading.PeriodicTimer _periodicTimer;

    public PeriodicTimer(System.Threading.PeriodicTimer periodicTimer)
    {
        _periodicTimer = periodicTimer;
    }

    public ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default)
        => _periodicTimer.WaitForNextTickAsync(cancellationToken);
}