
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
    private readonly IRentProjectionDataService _projectionDataService;

    private readonly IRentDataService _dataService;

    public RentProjectionFacade(IRentProjectionDataService projectionDataService, IRentDataService dataService)
    {
        _projectionDataService = projectionDataService;
        _dataService = dataService;
    }

    public async Task<Result<RentResponse>> CreateAsync(CreateRentProjectionEvent @event, CancellationToken cancellationToken = default)
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

    public async Task<Result<RentResponse>> UpdateAsync(UpdateRentProjectionEvent @event, CancellationToken cancellationToken = default)
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

    public async Task<Result<RentResponse>> GetAsync(Expression<Func<RentProjection, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entity = await _projectionDataService.GetAsync(predicate, cancellationToken);

        if(!entity.IsSuccess)
            return entity.Exception!;
        
        return entity.Value!.ToResponse();
    }
}
