using System.Linq.Expressions;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades;

public class EventFacade : IEventFacade
{
    private readonly IDataService<Event> _dataService;

    public EventFacade(IDataService<Event> dataService)
    {
        _dataService = dataService;
    }

    public async Task<Result<IEnumerable<EventResponse>>> FindAsync(Expression<Func<Event, bool>> predicate,
        bool descending = false,
        Expression<Func<Event, dynamic>>? orderBy = default,
        CancellationToken cancellationToken = default)
    {
        Result<IEnumerable<Event>>
            entities = await _dataService.FindAsync(predicate, false, orderBy, cancellationToken);

        if (!entities.IsSuccess)
        {
            return entities.Exception!;
        }

        return entities.Value?.Select(x => new EventResponse
        {
            Id = x.Id,
            SagaId = x.SagaId,
            StatusType = x.StatusType,
            SerializerType = x.SerializerType,
            Name = x.Name,
            Message = x.Message,
            Created = x.Created
        }).ToList() ?? Array.Empty<EventResponse>().ToList();
    }
}