using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Interfaces;

public interface IService<T> : ICreateService<T>, IDeleteService<T> where T : Entity
{
    Task<T?> GetAsync(string sql,
        IDictionary<string, dynamic> parameters,
        CancellationToken cancellationToken = default);
}