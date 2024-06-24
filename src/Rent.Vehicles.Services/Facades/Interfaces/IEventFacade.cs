using System.Linq.Expressions;

using LanguageExt.Common;

using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades.Interfaces;

public interface IEventFacade
{
    Task<Result<IEnumerable<EventResponse>>> FindAsync(Expression<Func<Entities.Event, bool>> predicate, CancellationToken cancellationToken = default);
}