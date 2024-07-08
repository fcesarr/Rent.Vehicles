using Rent.Vehicles.Consumers.Types;

namespace Rent.Vehicles.Consumers.Settings;

public record ConsumerSetting
{
    public ConsumerType Type { get; set; }

    public int BufferSize { get; set; }

    public IEnumerable<string> ToExcluded { get; set; } = [];

    public IEnumerable<string> ToIncluded { get; set; } = [];
}
