

using Rent.Vehicles.Entities;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades;

public class RentFacade : IRentFacade
{
    private readonly IDataService<Entities.Rent> _dataService;

    private readonly IDataService<RentalPlane> _rentalPlaneDataService;

    private readonly IVehicleDataService _vehicleService;

    private readonly IUnitOfWork _unitOfWork;

    public RentFacade(IDataService<Entities.Rent> dataService,
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
    
            var rentalPlanes = await _rentalPlaneDataService.GetAsync(x => x.Id == @event.RentPlaneId, cancellationToken);
    
            if(!rentalPlanes.IsSuccess)
                throw rentalPlanes.Exception!;
    
            var vehicle = await _vehicleService.GetAsync(x => !x.IsRented, cancellationToken);
    
            if(!vehicle.IsSuccess)
                throw vehicle.Exception!;
    
            var vehicleId = vehicle.Value;
    
            var today = DateTime.Now.Date;

            var startDate = today.AddDays(1);

            var entity = await _dataService.CreateAsync(new Entities.Rent
            {
                NumberOfDays = rentalPlanes.Value.NumberOfDays,
                DailyCost = rentalPlanes.Value.DailyCost,
                VehicleId = vehicleId.Id,
                UserId = @event.UserId,
                StartDate = startDate,
                PreEndDatePercentageFine = rentalPlanes.Value.PreEndDatePercentageFine,
                PostEndDateFine = rentalPlanes.Value.PostEndDateFine,
                EstimatedDate = startDate.AddDays(rentalPlanes.Value.NumberOfDays),
                EndDate = startDate.AddDays(rentalPlanes.Value.NumberOfDays),
                Cost = rentalPlanes.Value.DailyCost * rentalPlanes.Value.NumberOfDays
            });

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

}