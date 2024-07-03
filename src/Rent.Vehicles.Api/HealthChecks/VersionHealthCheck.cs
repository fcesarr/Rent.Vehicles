using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Rent.Vehicles.Api.HealthChecks;

[ExcludeFromCodeCoverage]
internal sealed class VersionHealthCheck : IHealthCheck
{
	public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
		CancellationToken cancellationToken = default)
	{
		var assembly = Assembly.GetExecutingAssembly();
		var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
		var version = fileVersionInfo.FileVersion ?? "1.0.0.0";

		var data = new Dictionary<string, object> { { "Version", version }, { "Hostname", Environment.MachineName } };

		var result = HealthCheckResult.Healthy(data: data);

		return Task.FromResult(result);
	}
}
