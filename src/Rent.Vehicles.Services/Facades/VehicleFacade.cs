
using Rent.Vehicles.Entities;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Types;
using Rent.Vehicles.Services.Extensions;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades;

public class VehicleFacade : IVehicleFacade
{
    private readonly IVehicleDataService _dataService;

    public VehicleFacade(IVehicleDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<Result<VehicleResponse>> CreateAsync(CreateVehiclesEvent @event, CancellationToken cancellationToken = default)
    {
        var entity = await _dataService.CreateAsync(@event.ToEntity(), cancellationToken);

        if(!entity.IsSuccess)
            return entity.Exception!;

        return entity.Value!.ToResponse();
    }
}
