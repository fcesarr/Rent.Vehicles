namespace Rent.Vehicles.Entities;

public interface IUnitOfWork
{
	Task BeginTransactionAsync(CancellationToken cancellationToken = default);

	Task CommitTransactionAsync(CancellationToken cancellationToken = default);

	Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
