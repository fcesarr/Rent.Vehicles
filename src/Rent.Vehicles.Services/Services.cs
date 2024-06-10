
using Microsoft.Extensions.Logging;

using Rent.Vehicles.Messages;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Services;

public sealed class Services<T> : IService<T> where T : Message
{
    private readonly ILogger<Services<T>> _logger;

    public Services(ILogger<Services<T>> logger)
    {
        _logger = logger;
    }

    public async Task Create(T message, CancellationToken cancellationToken = default)
        => await Task.Run(() =>  _logger.LogInformation("Create {obj}", message), cancellationToken);

}

