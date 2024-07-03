namespace Rent.Vehicles.Services.Interfaces;

public interface IUploadService
{
    Task<Result<string>> GetPathAsync(string base64String, CancellationToken cancellationToken = default);
    Task<Result<string>> UploadAsync(string base64String, CancellationToken cancellationToken = default);
}
