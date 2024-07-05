using Microsoft.Extensions.DependencyInjection;

using Rent.Vehicles.Consumers.IntegrationTests.Configuration;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;


[CollectionDefinition(nameof(CommonCollectionFixture))]
public class CommonCollectionFixture : ICollectionFixture<CommonFixture>
{
}


[CollectionDefinition(nameof(IntegrationTestWebAppFactoryFixture))]
public class IntegrationTestWebAppFactoryFixture : ICollectionFixture<IntegrationTestWebAppFactory>
{
}
