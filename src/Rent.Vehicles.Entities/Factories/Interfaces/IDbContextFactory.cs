using Rent.Vehicles.Entities.Contexts.Interfaces;

namespace Rent.Vehicles.Entities.Factories.Interfaces;

public interface IDbContextFactory
{
    Task<IDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default);
}
