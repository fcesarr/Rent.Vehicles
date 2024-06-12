using Rent.Vehicles.Consumers.RabbitMQ.Commands.BackgroundServices;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Messages.Commands;


namespace Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures.RabbitMQ.CollectionFixtures;

[CollectionDefinition(nameof(CreateVehiclesBackgroundServiceCollection))]
public class CreateVehiclesBackgroundServiceCollection :
    ICollectionFixture<CreateVehiclesBackgroundServiceFixture>
{

}

public class CreateVehiclesBackgroundServiceFixture : ConsumerFixture<CreateVehiclesCommandBackgroundService, CreateVehiclesCommand, Command>
{

}


