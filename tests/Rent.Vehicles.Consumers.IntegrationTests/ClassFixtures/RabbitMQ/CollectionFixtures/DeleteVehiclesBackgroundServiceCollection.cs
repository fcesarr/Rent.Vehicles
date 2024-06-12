using Rent.Vehicles.Consumers.RabbitMQ.Commands.BackgroundServices;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Messages.Commands;


namespace Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures.RabbitMQ.CollectionFixtures;

[CollectionDefinition(nameof(DeleteVehiclesBackgroundServiceCollection))]
public class DeleteVehiclesBackgroundServiceCollection :
    ICollectionFixture<DeleteVehiclesBackgroundServiceFixture>
{

}

public class DeleteVehiclesBackgroundServiceFixture : ConsumerFixture<DeleteVehiclesCommandBackgroundService, DeleteVehiclesCommand, Command>
{

}


