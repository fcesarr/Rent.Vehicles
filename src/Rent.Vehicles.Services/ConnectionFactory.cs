using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace Rent.Vehicles.Services;

public interface IConnectionFactory
{
    Task<IDbConnection> GetConnectionAsync(CancellationToken cancellationToken = default);
}

public sealed class ConnectionFactory<T> : IConnectionFactory where T : DbConnection, new()
{
    private readonly string _connectionString;

    public ConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IDbConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
    {
        return await Task.Run<IDbConnection>(() => {

            T connection = new()
            {
                ConnectionString = _connectionString
            };

            return connection;

        }, cancellationToken);
    }

}