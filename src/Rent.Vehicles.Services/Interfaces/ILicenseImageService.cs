using LanguageExt.Common;

namespace Rent.Vehicles.Services.Interfaces;

public interface ILicenseImageService : IAction<Task>
{
    Task<Result<string>> GetPathAsync(string licenseImage, CancellationToken cancellationToken = default);

    Task<Result<Task>> UploadAsync(string licenseImage, CancellationToken cancellationToken = default);
}