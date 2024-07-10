using System.Diagnostics.CodeAnalysis;

namespace Rent.Vehicles.Lib.Constants;

[ExcludeFromCodeCoverage]
public static class HealthCheckUri
{
    public const string Ready = "/health/ready";

    public const string Live = "/health/live";
}
