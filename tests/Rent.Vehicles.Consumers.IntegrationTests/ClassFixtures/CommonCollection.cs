using Microsoft.Extensions.DependencyInjection;

using Rent.Vehicles.Consumers.IntegrationTests.Configuration;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;


[CollectionDefinition(nameof(CommonCollection))]
public class CommonCollection : ICollectionFixture<CommonFixture>
{
}
