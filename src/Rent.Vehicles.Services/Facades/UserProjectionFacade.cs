using System.Linq.Expressions;

using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Extensions;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades;

public class UserProjectionFacade : IUserProjectionFacade
{
    private readonly IUserProjectionDataService _dataService;

    private readonly IUserDataService _userDataService;

    public UserProjectionFacade(IUserProjectionDataService dataService, IUserDataService userDataService)
    {
        _dataService = dataService;
        _userDataService = userDataService;
    }

    public async Task<Result<UserResponse>> CreateAsync(CreateUserProjectionEvent @event,
        CancellationToken cancellationToken = default)
    {
        var user = await _userDataService.GetAsync(x => x.Id == @event.Id, cancellationToken);

        if (!user.IsSuccess)
        {
            return user.Exception!;
        }

        var entity =
            await _dataService.CreateAsync(user.Value!.ToProjection(), cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return entity.Value!.ToResponse();
    }

    public async Task<Result<UserResponse>> GetAsync(Expression<Func<UserProjection, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dataService.GetAsync(predicate, cancellationToken);

        if(!entity.IsSuccess)
            return entity.Exception!;
        
        return entity.Value!.ToResponse();
    }

    public async Task<Result<UserResponse>> UpdateAsync(UpdateUserProjectionEvent @event,
        CancellationToken cancellationToken = default)
    {
        var user = await _userDataService.GetAsync(x => x.Id == @event.Id, cancellationToken);

        if (!user.IsSuccess)
        {
            return user.Exception!;
        }

        var entity =
            await _dataService.UpdateAsync(user.Value!.ToProjection(), cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return entity.Value!.ToResponse();
    }
}
