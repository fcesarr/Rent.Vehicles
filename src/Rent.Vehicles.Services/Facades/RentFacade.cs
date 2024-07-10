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

    private readonly IRentalPlaneDataService _rentalPlaneDataService;

    private readonly IUnitOfWork _unitOfWork;

    private readonly IUserDataService _userService;

    private readonly IVehicleDataService _vehicleService;

    public RentFacade(IRentDataService dataService, IRentalPlaneDataService rentalPlaneDataService,
        IUnitOfWork unitOfWork, IVehicleDataService vehicleService, IUserDataService userService)
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

            if (!user.IsSuccess)
            {
                throw user.Exception!;
            }

            var vehicle = await _vehicleService.RentItAsync(cancellationToken);

            if (!vehicle.IsSuccess)
            {
                throw vehicle.Exception!;
            }

            var entity = await _dataService.CreateAsync(rentalPlane.Value!,
                @event.Id,
                @event.UserId,
                vehicle.Value!.Id,
                cancellationToken);

            if (!entity.IsSuccess)
            {
                throw entity.Exception!;
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

    public async Task<Result<RentResponse>> UpdateAsync(UpdateRentEvent @event,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var entity = await _dataService.UpdateAsync(@event.Id, @event.EstimatedDate, cancellationToken);

            if (!entity.IsSuccess)
            {
                throw entity.Exception!;
            }

            var user = await _userService.GetAsync(x => x.Id == entity.Value!.UserId, cancellationToken);

            if (!user.IsSuccess)
            {
                throw user.Exception!;
            }

            var vehicle = await _vehicleService.ReturnItAsync(entity.Value!.VehicleId, cancellationToken);

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
