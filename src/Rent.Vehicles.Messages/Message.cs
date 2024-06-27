using System.Text.Json.Serialization;

using MessagePack;

namespace Rent.Vehicles.Messages;

[MessagePackObject]
public abstract record Message
{
    [Key(0)]
    [JsonIgnore]
    public Guid SagaId
    {
        get;
        set;
    }
}