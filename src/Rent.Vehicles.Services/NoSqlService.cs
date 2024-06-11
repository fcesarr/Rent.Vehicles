

using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Services;

public sealed class NoSqlService<T> : IService<T> where T : Entity
{
    private readonly ILogger<NoSqlService<T>> _logger;

    public NoSqlService(ILogger<NoSqlService<T>> logger)
    {
        _logger = logger;
    }

    public async Task CreateAsync(T? entity, CancellationToken cancellationToken = default)
    {
        if(entity == null)
            return;

        await Task.Run(() => _logger.LogInformation("Create {obj}", entity.Id), cancellationToken);
    }

    public async Task DeleteAsync(T? entity, CancellationToken cancellationToken = default)
    {
        if(entity == null)
            return;

        await Task.Run(() => _logger.LogInformation("Delete {obj}", entity.Id), cancellationToken);
    }

    public async Task<T?> GetAsync(string sql,
        IDictionary<string, dynamic> parameters,
        CancellationToken cancellationToken = default)
    {
       return await Task.Run(() => default(T), cancellationToken);
    }
}

