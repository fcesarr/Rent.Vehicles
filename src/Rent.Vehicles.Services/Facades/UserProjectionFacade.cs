using System.Linq.Expressions;

using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Messages.Types;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Extensions;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades;

public class UserProjectionFacade : IUserProjectionFacade
{
    private readonly IUserProjectionDataService _dataService;

    public UserProjectionFacade(IUserProjectionDataService dataService)
    {
        _dataService = dataService;
    }

    public Task<Result<UserResponse>> CreateAsync(CreateUserProjectionEvent @event,
        CancellationToken cancellationToken = default)
    {
       throw new NotImplementedException();
    }

    public Task<Result<UserResponse>> GetAsync(Expression<Func<UserProjection, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<UserResponse>> UpdateAsync(UpdateUserProjectionEvent @event,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
