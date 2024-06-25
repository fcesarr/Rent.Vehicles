using System.Linq.Expressions;

using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades.Interfaces;

public interface IEventFacade
{
    Task<Result<IEnumerable<EventResponse>>> FindAsync(Expression<Func<Entities.Event, bool>> predicate,
        bool descending = false,
        Expression<Func<Entities.Event, dynamic>>? orderBy = default,
        CancellationToken cancellationToken = default);
}