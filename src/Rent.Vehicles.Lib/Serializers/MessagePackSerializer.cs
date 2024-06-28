using MessagePack;

using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Lib.Types;

namespace Rent.Vehicles.Lib.Serializers;

public class MessagePackSerializer : ISerializer
{
    private readonly MessagePackSerializerOptions _options;

    public MessagePackSerializer(MessagePackSerializerOptions? options = default)
    {
        _options = options ?? MessagePackSerializerOptions.Standard;
    }

    public SerializerType SerializerType => SerializerType.MessagePack;

    public async Task<T?> DeserializeAsync<T>(byte[] bytes, CancellationToken cancellationToken = default)
        where T : class
    {
        using MemoryStream inputStream = new(bytes);

        return await MessagePack.MessagePackSerializer.DeserializeAsync<T>(inputStream, _options, cancellationToken);
    }

    public async Task<byte[]> SerializeAsync<T>(T? entity, CancellationToken cancellationToken = default)
        where T : class
    {
        if (entity is null)
        {
            return [];
        }

        using MemoryStream inputStream = new();

        await MessagePack.MessagePackSerializer.SerializeAsync(inputStream, entity, _options, cancellationToken);

        return inputStream.ToArray();
    }

    public async Task<byte[]> SerializeAsync(dynamic? entity, Type type, CancellationToken cancellationToken = default)
    {
        if (entity is null)
        {
            return [];
        }

        using MemoryStream inputStream = new();

        await MessagePack.MessagePackSerializer.SerializeAsync(type, inputStream, entity, _options, cancellationToken);

        return inputStream.ToArray();
    }
}
