using Rent.Vehicles.Lib.Types;

namespace Rent.Vehicles.Lib.Serializers.Interfaces;

public interface ISerializer
{
    SerializerType SerializerType
    {
        get;
    }

    Task<byte[]> SerializeAsync<T>(T? entity,
        CancellationToken cancellationToken = default) where T : class;

    Task<byte[]> SerializeAsync(dynamic? entity, Type type,
        CancellationToken cancellationToken = default);

    Task<T?> DeserializeAsync<T>(byte[] bytes,
        CancellationToken cancellationToken = default) where T : class;
}
