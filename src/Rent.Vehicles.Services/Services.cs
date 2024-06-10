
using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Services;

public sealed class Services<T> : IService<T> where T : Entity
{
    private readonly ILogger<Services<T>> _logger;

    public Services(ILogger<Services<T>> logger)
    {
        _logger = logger;
    }

    public async Task Create(T? entity, CancellationToken cancellationToken = default)
        => await Task.Run(() =>  _logger.LogInformation("Create {obj}", entity), cancellationToken);

}

