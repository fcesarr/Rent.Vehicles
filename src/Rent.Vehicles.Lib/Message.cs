using System.Text.Json.Serialization;

using MessagePack;

namespace Rent.Vehicles.Lib;

[MessagePackObject]
public abstract record Message
{
    private Guid _sagaId = Guid.NewGuid();

    [Key(0)]
    [JsonIgnore]
    public Guid SagaId
    {
        get => _sagaId;
        set => _sagaId = value == default ? _sagaId : value;
    }
}
