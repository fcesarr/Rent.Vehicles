

using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;

namespace Rent.Vehicles.Services;

public sealed class Service<T> : IService<T> where T : Entity
{
    private readonly ILogger<Service<T>> _logger;

    private readonly IRepository<T> _repository;

    public Service(ILogger<Service<T>> logger, IRepository<T> repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task CreateAsync(T? entity, CancellationToken cancellationToken = default)
    {
        if(entity == null)
            return;

        await _repository.CreateAsync(entity, cancellationToken);
    }

    public async Task<T?> GetAsync(string sql,
        IDictionary<string, dynamic> parameters,
        CancellationToken cancellationToken = default)
    {
       return await _repository.GetAsync(sql, parameters, cancellationToken);
    }
}

