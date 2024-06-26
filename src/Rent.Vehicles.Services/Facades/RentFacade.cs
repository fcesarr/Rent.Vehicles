

using Rent.Vehicles.Entities;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades;

public class RentFacade : IRentFacade
{
    private readonly IRentDataService _dataService;

    private readonly IDataService<RentalPlane> _rentalPlaneDataService;

    private readonly IVehicleDataService _vehicleService;

    private readonly IUnitOfWork _unitOfWork;

    public RentFacade(IRentDataService dataService,
        IDataService<RentalPlane> rentalPlaneDataService,
        IVehicleDataService vehicleService,
        IUnitOfWork unitOfWork)
    {
        _dataService = dataService;
        _rentalPlaneDataService = rentalPlaneDataService;
        _vehicleService = vehicleService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RentResponse>> CreateAsync(CreateRentEvent @event, CancellationToken cancellationToken = default)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
    
            var rentalPlane = await _rentalPlaneDataService.GetAsync(x => x.Id == @event.RentPlaneId, cancellationToken);
    
            if(!rentalPlane.IsSuccess)
                throw rentalPlane.Exception!;
    
            var vehicle = await _vehicleService.GetAsync(x => !x.IsRented, cancellationToken);
    
            if(!vehicle.IsSuccess)
                throw vehicle.Exception!;
    
            var entity = await _dataService.CreateAsync(rentalPlane.Value,
                @event.UserId,
                vehicle.Value.Id, 
                cancellationToken);

            if(!entity.IsSuccess)
                throw entity.Exception!;
    
            vehicle.Value.IsRented = true;
    
            vehicle = await _vehicleService.UpdateAsync(vehicle.Value, cancellationToken);

            if(!vehicle.IsSuccess)
                throw vehicle.Exception!;
    
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
    
            return new RentResponse();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return ex;
        }
    }

    public async Task<Result<CostResponse>> EstimateCostAsync(Guid id, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var entity = await _dataService.EstimateCostAsync(id, endDate, cancellationToken);

        if(!entity.IsSuccess)
            return entity.Exception!;

        return new CostResponse(entity.Value.Cost, entity.Value.EndDate);
    }

    public async Task<Result<RentResponse>> UpdateAsync(UpdateRentEvent @event, CancellationToken cancellationToken = default)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var entity = await _dataService.UpdateAsync(@event.Id, @event.EndDate, cancellationToken);
                
            var vehicle = await _vehicleService.GetAsync(x => x.Id == entity.Value.VehicleId, cancellationToken);

            if(!vehicle.IsSuccess)
                throw entity.Exception!;

            vehicle.Value.IsRented = false;

            await _vehicleService.UpdateAsync(vehicle.Value, cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
    
            return new RentResponse();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return ex;
        }
    }
}