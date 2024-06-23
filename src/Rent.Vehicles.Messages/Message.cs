using System.Text.Json.Serialization;

using MessagePack;

namespace Rent.Vehicles.Messages;

[MessagePackObject]
public abstract record Message
{
    [Key(0)]
    [JsonIgnore]
    public required Guid SagaId { get; set; }
}