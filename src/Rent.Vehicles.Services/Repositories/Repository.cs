
using System.Data;

using Dapper;

using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Repositories.Interfaces;

namespace Rent.Vehicles.Services.Repositories;

public sealed class Repository<T> : IRepository<T> where T : Entity
{
    private readonly ILogger<Repository<T>> _logger;

    private readonly IDictionary<string, string> _sqls;

    private readonly IConnectionFactory _connectionFactory;

    public Repository(ILogger<Repository<T>> logger,
        IDictionary<string, string> sqls,
        IConnectionFactory connectionFactory)
    {
        _logger = logger;
        _sqls = sqls;
        _connectionFactory = connectionFactory;
    }

    public async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        if(_sqls.TryGetValue($"Insert{typeof(T).Name}.sql", out string? sql))
        {
            if(cancellationToken.IsCancellationRequested)
                return;

            var connection = await _connectionFactory.GetConnectionAsync(cancellationToken);

            connection.Open();

            await connection.ExecuteAsync(sql, entity);
            
            _logger.LogInformation("Create {obj}", entity.Id);

            connection.Close();
        }

    }

    public async Task<T?> GetAsync(string sql,
        IDictionary<string, dynamic> parameters,
        CancellationToken cancellationToken = default)
    {   
        var connection = await _connectionFactory.GetConnectionAsync(cancellationToken);

        connection.Open();

        var dynamicParameters = new DynamicParameters(parameters);

        var entity = await connection.QueryFirstOrDefaultAsync<T>(sql, dynamicParameters);

        connection.Close();

        return entity;
    }
}