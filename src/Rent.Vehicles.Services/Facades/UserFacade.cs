using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Services.Extensions;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades;

public class UserFacade : IUserFacade
{
    private readonly IUserDataService _dataService;
    private readonly IUploadService _uploadService;

    public UserFacade(IUserDataService dataService, IUploadService uploadService)
    {
        _dataService = dataService;
        _uploadService = uploadService;
    }

    public async Task<Result<UserResponse>> CreateAsync(CreateUserEvent @event,
        CancellationToken cancellationToken = default)
    {
        var licensePath =
            await _uploadService.GetPathAsync(@event.LicenseImage, cancellationToken);

        if (!licensePath.IsSuccess)
        {
            return licensePath.Exception!;
        }

        var entity = await _dataService.CreateAsync(@event.ToEntity(licensePath.Value!), cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return entity.Value!.ToResponse();
    }

    public async Task<Result<UserResponse>> UpdateAsync(UpdateUserLicenseImageEvent @event,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dataService.GetAsync(x => x.Id == @event.Id, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        var licensePath =
            await _uploadService.GetPathAsync(@event.LicenseImage, cancellationToken);

        if (!licensePath.IsSuccess)
        {
            return licensePath.Exception!;
        }

        entity.Value!.LicensePath = licensePath.Value!;

        var entityResult = await _dataService.UpdateAsync(entity.Value!, cancellationToken);

        if (!entityResult.IsSuccess)
        {
            return entityResult.Exception!;
        }

        return entityResult.Value!.ToResponse();
    }

    public async Task<Result<UserResponse>> UpdateAsync(UpdateUserEvent @event,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dataService.GetAsync(x => x.Id == @event.Id, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        var entityToUpdate = @event.ToEntity(entity.Value!);

        var entityResult = await _dataService.UpdateAsync(entityToUpdate, cancellationToken);

        if (!entityResult.IsSuccess)
        {
            return entityResult.Exception!;
        }

        return entityResult.Value!.ToResponse();
    }
}
