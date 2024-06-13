


using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;

namespace Rent.Vehicles.Services;

public sealed class NoSqlService<T> : IService<T> where T : Entity
{
    private readonly ILogger<NoSqlService<T>> _logger; 

    private readonly IMongoRepository<T> _mongoRepository;

    public NoSqlService(ILogger<NoSqlService<T>> logger,
        IMongoRepository<T> mongoRepository)
    {
        _logger = logger;
        _mongoRepository = mongoRepository;
    }

    public async Task CreateAsync(T? entity, CancellationToken cancellationToken = default)
    {
        if(entity == null)
            return;

        await _mongoRepository.CreateAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await Task.Run(() => _logger.LogInformation("Delete {obj}", id), cancellationToken);
    }

    public async Task<T?> GetAsync(string sql,
        IDictionary<string, dynamic> parameters,
        CancellationToken cancellationToken = default)
    {
       return await Task.Run(() => default(T), cancellationToken);
    }

    public Task UpdateAsync(T? entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

