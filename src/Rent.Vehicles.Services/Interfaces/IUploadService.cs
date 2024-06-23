using LanguageExt.Common;

namespace Rent.Vehicles.Services.Interfaces;

public interface IUploadService
{
    Task<Result<Task>> UploadAsync(string filePath, byte[] fileBytes, CancellationToken cancellationToken = default);

}