using System.Linq.Expressions;

using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Extensions;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades;

public class RentProjectionFacade : IRentProjectionFacade
{
    private readonly IRentDataService _dataService;
    private readonly IRentProjectionDataService _projectionDataService;
    private readonly IRentalPlaneDataService _rentalPlaneDataService;

    public RentProjectionFacade(IRentDataService dataService, IRentProjectionDataService projectionDataService, IRentalPlaneDataService rentalPlaneDataService)
    {
        _dataService = dataService;
        _projectionDataService = projectionDataService;
        _rentalPlaneDataService = rentalPlaneDataService;
    }

    public async Task<Result<RentResponse>> CreateAsync(CreateRentProjectionEvent @event,
        CancellationToken cancellationToken = default)
    {
        var rent = await _dataService.GetAsync(x => x.Id == @event.Id, cancellationToken);

        if (!rent.IsSuccess)
        {
            return rent.Exception!;
        }

        var entity = await _projectionDataService.CreateAsync(rent.Value!.ToProjection(),
            cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return entity.Value!.ToResponse();
    }

    public async Task<Result<RentResponse>> UpdateAsync(UpdateRentProjectionEvent @event,
        CancellationToken cancellationToken = default)
    {
        var rent = await _dataService.GetAsync(x => x.Id == @event.Id, cancellationToken);

        if (!rent.IsSuccess)
        {
            return rent.Exception!;
        }

        var entity = await _projectionDataService.UpdateAsync(rent.Value!.ToProjection(),
            cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return entity.Value!.ToResponse();
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

    public async Task<Result<RentResponse>> GetAsync(Expression<Func<RentProjection, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var entity = await _projectionDataService.GetAsync(predicate, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return entity.Value!.ToResponse();
    }

    public async Task<Result<IEnumerable<RentalPlaneResponse>>> FindAllRentalPlanesAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _rentalPlaneDataService.FindAsync(x => true, cancellationToken: cancellationToken);

        if(!entities.IsSuccess)
        {
            return entities.Exception!;
        }

        return entities.Value!
            .Select(x => x.ToResponse())
            .ToList();
    }
}
