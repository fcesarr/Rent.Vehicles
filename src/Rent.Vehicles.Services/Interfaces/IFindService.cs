using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Interfaces;

public interface IFindService<T> where T : Entity
{
    Task<IEnumerable<T>> FindAsync(Guid sagaId, CancellationToken cancellationToken = default);
}