using LanguageExt.Common;

using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Dtos;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services;

public class UserFacade : IUserFacade
{
    private readonly ILogger<UserFacade> _logger;

    private readonly IValidator<UserDto> _validator;

    private readonly IUserService _userService;

    public UserFacade(ILogger<UserFacade> logger,
        IValidator<UserDto> validator,
        IUserService userService)
    {
        _logger = logger;
        _validator = validator;
        _userService = userService;
    }

    public async Task<Result<User>> CreateAsync(UserDto? dto, CancellationToken cancellationToken = default)
    {
        var result = await _validator.ValidateAsync(dto, cancellationToken);

        if(!result.IsValid)
            return new Result<User>(result.Exception);

        return await _userService.CreateAsync(new User {
            Name = result.Instance.Name,
            Number = result.Instance.Number,
            Birthday = result.Instance.Birthday,
            LicenseNumber = result.Instance.LicenseNumber,
            LicenseType = result.Instance.LicenseType,
            LicensePath = await GetPathAsync(result.Instance.LicenseImage)
        }, cancellationToken);
    }

    public async Task<Result<User>> UpdateAsync(Guid id, string licenseImage, CancellationToken cancellationToken = default)
    {
        var entity = await _userService.GetAsync(x => x.Id == id, cancellationToken);

        return await entity.Match(async entity => {

            var dto = new UserDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Number = entity.Number,
                Birthday = entity.Birthday,
                LicenseNumber = entity.LicenseNumber,
                LicenseType = entity.LicenseType,
                LicenseImage = licenseImage,
            };

            var result = await _validator.ValidateAsync(dto, cancellationToken);

            if(!result.IsValid)
                return new Result<User>(result.Exception);
            
            entity.LicensePath = await GetPathAsync(licenseImage);

            return entity;
        }, exception => Task.FromResult(new Result<User>(exception)));
    }

    private Task<string> GetPathAsync(string licenseImage)
    {
        return Task.FromResult(string.Empty);
    }
}