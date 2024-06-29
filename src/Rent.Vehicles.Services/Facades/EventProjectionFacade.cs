using System.Linq.Expressions;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Extensions;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades;

public class EventProjectionFacade : IEventProjectionFacade
{
    private readonly IEventProjectionDataService _projectionDataService;

    private readonly IEventDataService _dataService;

    public EventProjectionFacade(IEventProjectionDataService projectionDataService, IEventDataService dataService)
    {
        _projectionDataService = projectionDataService;
        _dataService = dataService;
    }

    public async Task<Result<EventResponse>> CreateAsync(EventProjectionEvent @event, CancellationToken cancellationToken = default)
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

    public async Task<Result<IEnumerable<EventResponse>>> FindAsync(Expression<Func<EventProjection, bool>> predicate,
        bool descending = false,
        Expression<Func<EventProjection, dynamic>>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        var entities = await _projectionDataService.FindAsync(predicate, false, orderBy, cancellationToken);

        if (!entities.IsSuccess)
        {
            return entities.Exception!;
        }

        return entities.Value?.Select(x => x.ToResponse()).ToList() ?? Array.Empty<EventResponse>().ToList();
    }
}
