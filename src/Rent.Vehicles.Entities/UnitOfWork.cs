using Rent.Vehicles.Entities.Contexts.Interfaces;

namespace Rent.Vehicles.Entities;

public class UnitOfWork : IUnitOfWork
{
    private readonly IUnitOfWorkerContext _context;

    private readonly IDictionary<Type, IRepository> _repositories;

    public UnitOfWork(IEnumerable<IRepository> repositories, IUnitOfWorkerContext context)
    {
        _repositories = repositories.ToDictionary(x => x.GetType(), x => x);
        _context = context;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _context.BeginTransactionAsync(cancellationToken);

        foreach (var repository in _repositories.Values)
        {
            repository.SetContext(_context);
        }
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _context.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _context.RollbackTransactionAsync(cancellationToken);
    }
}
