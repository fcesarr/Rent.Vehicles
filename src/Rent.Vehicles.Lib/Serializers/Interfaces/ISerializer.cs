namespace Rent.Vehicles.Lib.Serializers.Interfaces;

public interface ISerializer
{
    Task<byte[]> SerializeAsync<T>(T? entity,
        CancellationToken cancellationToken = default) where T : class;
    
    Task<T?> DeserializeAsync<T>(byte[] bytes,
        CancellationToken cancellationToken = default) where T : class;
}