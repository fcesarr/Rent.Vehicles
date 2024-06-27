using System.Linq.Expressions;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades.Interfaces;

public interface IEventFacade
{
    Task<Result<IEnumerable<EventResponse>>> FindAsync(Expression<Func<Event, bool>> predicate,
        bool descending = false,
        Expression<Func<Event, dynamic>>? orderBy = default,
        CancellationToken cancellationToken = default);
}
