using System.Linq.Expressions;

using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Extensions;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades;

public class EventFacade : IEventFacade
{
    private readonly IEventDataService _dataService;

    private readonly ISerializer _serializer;

    public EventFacade(IEventDataService dataService, ISerializer serializer)
    {
        _dataService = dataService;
        _serializer = serializer;
    }

    public async Task<Result<EventResponse>> CreateAsync(Event @event,
        CancellationToken cancellationToken = default)
    {
        var data = await _serializer.SerializeAsync(@event, cancellationToken);

        var entity = await _dataService.CreateAsync(@event.ToEntity(data), cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return entity.Value!.ToResponse();
    }

    public async Task<Result<IEnumerable<EventResponse>>> FindAsync(Expression<Func<Entities.Event, bool>> predicate,
        bool descending = false,
        Expression<Func<Entities.Event, dynamic>>? orderBy = default,
        CancellationToken cancellationToken = default)
    {
        var entities = await _dataService.FindAsync(predicate, false, orderBy, cancellationToken);

        if (!entities.IsSuccess)
        {
            return entities.Exception!;
        }

        return entities.Value?.Select(x => x.ToResponse()).ToList() ?? Array.Empty<EventResponse>().ToList();
    }
}
