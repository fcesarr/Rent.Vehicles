using System.Linq.Expressions;

using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades.Interfaces;

public interface IEventProjectionFacade : IFacade
{
    Task<Result<EventResponse>> CreateAsync(EventProjectionEvent @event, CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<EventResponse>>> FindAsync(Expression<Func<EventProjection, bool>> predicate,
        bool descending = false,
        Expression<Func<EventProjection, dynamic>>? orderBy = default,
        CancellationToken cancellationToken = default);
}
