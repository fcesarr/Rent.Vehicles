using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Interfaces;

public interface IVehiclesService : ISqlVehiclesService, INoSqlVehiclesService;

public interface ISqlVehiclesService :  ISqlService<Vehicle>
{
    Task UpdateAsync(Guid id, string licensePlate, CancellationToken cancellationToken = default);
}


public interface INoSqlVehiclesService : INoSqlService<Vehicle>
{
    Task UpdateAsync(Guid id, string licensePlate, CancellationToken cancellationToken = default);
}