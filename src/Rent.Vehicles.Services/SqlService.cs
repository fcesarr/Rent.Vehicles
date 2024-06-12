


using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;

namespace Rent.Vehicles.Services;

public sealed class SqlService<T> : IService<T> where T : Entity
{
    private readonly ILogger<NoSqlService<T>> _logger;

    private readonly IRepository<T> _repository;

    public SqlService(ILogger<NoSqlService<T>> logger, IRepository<T> repository)
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

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<T?> GetAsync(string sql,
        IDictionary<string, dynamic> parameters,
        CancellationToken cancellationToken = default)
    {
       return await _repository.GetAsync(sql, parameters, cancellationToken);
    }
}

