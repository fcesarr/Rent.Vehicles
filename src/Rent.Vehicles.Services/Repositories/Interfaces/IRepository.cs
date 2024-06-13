using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Repositories.Interfaces;

public interface IRepository<T> where T : Entity
{
    Task CreateAsync(T entity, CancellationToken cancellationToken = default);

    Task<T?> GetAsync(Guid sagaId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> FindAsync(Guid sagaId,
        CancellationToken cancellationToken = default);
}