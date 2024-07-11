using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Extensions;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades;

public class VehicleFacade : IVehicleFacade
{
    private readonly IVehicleDataService _dataService;

    private readonly IRentDataService _rentDataService;

    public VehicleFacade(IVehicleDataService dataService, IRentDataService rentDataService)
    {
        _dataService = dataService;
        _rentDataService = rentDataService;
    }

    public async Task<Result<VehicleResponse>> CreateAsync(CreateVehiclesEvent @event,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dataService.CreateAsync(@event.ToEntity(), cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return entity.Value!.ToResponse();
    }

    public async Task<Result<bool>> DeleteAsync(DeleteVehiclesEvent @event,
        CancellationToken cancellationToken = default)
    {
        var rent = await _rentDataService.GetAsync(x =>x.VehicleId == @event.Id,
            cancellationToken: cancellationToken);

        if(rent.IsSuccess)
        {
            return new Exception("Veiculo possui alugueis cadastrados");
        }

        var entity = await _dataService.DeleteAsync(@event.Id, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return entity.Value;
    }

    public async Task<Result<VehicleResponse>> UpdateAsync(UpdateVehiclesEvent @event,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dataService.UpdateAsync(@event.Id, @event.LicensePlate, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return entity.Value!.ToResponse();
    }
}
