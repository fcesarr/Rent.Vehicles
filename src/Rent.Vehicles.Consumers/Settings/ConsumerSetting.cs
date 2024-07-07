using Rent.Vehicles.Consumers.Types;

namespace Rent.Vehicles.Consumers.Settings;

public record ConsumerSetting
{
    public ConsumerType Type { get; set; }
}
