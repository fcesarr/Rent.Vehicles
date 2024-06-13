using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Interfaces;

public interface IService<T> :
    ICreateService<T>,
    IDeleteService<T>,
    IUpdateService<T>,
    IGetService<T>,
    IFindService<T> where T : Entity
{
    Task<T?> GetAsync(Guid sagaId,
        CancellationToken cancellationToken = default);
}