using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Interfaces;

public interface IVehiclesService : ISqlService<Vehicle>, INoSqlService<Vehicle>
{
    Task UpdateAsync(Guid id, string licensePlate, CancellationToken cancellationToken = default);
}