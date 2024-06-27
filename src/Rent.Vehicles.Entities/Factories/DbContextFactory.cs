using Microsoft.EntityFrameworkCore;

using Rent.Vehicles.Entities.Contexts.Interfaces;
using Rent.Vehicles.Entities.Factories.Interfaces;

namespace Rent.Vehicles.Entities.Factories;

public class DbContextFactory<TImplementation> : IDbContextFactory
    where TImplementation : DbContext, IDbContext
{
    private readonly IDbContextFactory<TImplementation> _dbContextFactory;

    public DbContextFactory(IDbContextFactory<TImplementation> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContextFactory.CreateDbContextAsync(cancellationToken);
    }
}
