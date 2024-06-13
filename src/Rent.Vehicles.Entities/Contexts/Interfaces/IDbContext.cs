using Microsoft.EntityFrameworkCore;

namespace Rent.Vehicles.Entities.Contexts.Interfaces;

public interface IDbContext : IDisposable
{
	public DbSet<TEntity> Set<TEntity>() where TEntity : class;
}