using Rent.Vehicles.Entities;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Extensions;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades;

public class RentFacade : IRentFacade
{
    private readonly IRentDataService _dataService;

    private readonly IDataService<RentalPlane> _rentalPlaneDataService;

    private readonly IUnitOfWork _unitOfWork;

    private readonly IVehicleDataService _vehicleService;

    private readonly IUserDataService _userService;

    public RentFacade(IRentDataService dataService, IDataService<RentalPlane> rentalPlaneDataService, IUnitOfWork unitOfWork, IVehicleDataService vehicleService, IUserDataService userService)
    {
        _dataService = dataService;
        _rentalPlaneDataService = rentalPlaneDataService;
        _unitOfWork = unitOfWork;
        _vehicleService = vehicleService;
        _userService = userService;
    }

    public async Task<Result<RentResponse>> CreateAsync(CreateRentEvent @event,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var rentalPlane =
                await _rentalPlaneDataService.GetAsync(x => x.Id == @event.RentPlaneId, cancellationToken);

            if (!rentalPlane.IsSuccess)
            {
                throw rentalPlane.Exception!;
            }

            var user = await _userService.GetAsync(x => x.Id == @event.UserId, cancellationToken);

            if(!user.IsSuccess)
                throw user.Exception!;

            var vehicle = await _vehicleService.GetAsync(x => !x.IsRented, cancellationToken);

            if (!vehicle.IsSuccess)
            {
                throw vehicle.Exception!;
            }

            var entity = await _dataService.CreateAsync(rentalPlane.Value!,
                @event.UserId,
                vehicle.Value!.Id,
                cancellationToken);

            if (!entity.IsSuccess)
            {
                throw entity.Exception!;
            }

            vehicle.Value!.IsRented = true;

            vehicle = await _vehicleService.UpdateAsync(vehicle.Value!, cancellationToken);

            if (!vehicle.IsSuccess)
            {
                throw vehicle.Exception!;
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return entity.Value!.ToResponse(vehicle.Value!, user.Value!);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return ex;
        }
    }

    public async Task<Result<CostResponse>> EstimateCostAsync(Guid id, DateTime estimatedDate,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dataService.EstimateCostAsync(id, estimatedDate, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return new CostResponse(entity.Value!.Cost);
    }

    public async Task<Result<RentResponse>> UpdateAsync(UpdateRentEvent @event,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var entity = await _dataService.UpdateAsync(@event.Id, @event.EstimatedDate, cancellationToken);

            var user = await _userService.GetAsync(x => x.Id == entity.Value!.UserId, cancellationToken);

            if(!user.IsSuccess)
                throw user.Exception!;

            var vehicle =
                await _vehicleService.GetAsync(x => x.Id == entity.Value!.VehicleId, cancellationToken);

            if (!vehicle.IsSuccess)
            {
                throw entity.Exception!;
            }

            vehicle.Value!.IsRented = true;

            vehicle = await _vehicleService.UpdateAsync(vehicle.Value!, cancellationToken);

            if (!vehicle.IsSuccess)
            {
                throw vehicle.Exception!;
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return entity.Value!.ToResponse(vehicle.Value!, user.Value!);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return ex;
        }
    }
}
