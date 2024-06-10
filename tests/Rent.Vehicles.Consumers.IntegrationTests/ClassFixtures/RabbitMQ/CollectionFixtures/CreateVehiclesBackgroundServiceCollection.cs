using Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;
using Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices;
using Rent.Vehicles.Messages;

using Xunit;


[CollectionDefinition(nameof(CreateVehiclesBackgroundServiceCollection))]
public class CreateVehiclesBackgroundServiceCollection :
    ICollectionFixture<CreateVehiclesBackgroundServiceFixture>
{

}

public class CreateVehiclesBackgroundServiceFixture : ConsumerFixture<CreateBackgroundService<CreateVehiclesCommand>, CreateVehiclesCommand>
{

}


