namespace Rent.Vehicles.Entities.Contexts.Interfaces;

public interface IUnitOfWorkerContext : IDbContext, IAsyncDisposable
{
	Task BeginTransactionAsync(CancellationToken cancellationToken = default);

	Task CommitTransactionAsync(CancellationToken cancellationToken = default);

	Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
