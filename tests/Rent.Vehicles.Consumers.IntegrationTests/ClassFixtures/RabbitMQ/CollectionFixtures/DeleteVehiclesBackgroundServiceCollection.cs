using Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;
using Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Messages.Commands;


namespace Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures.RabbitMQ.CollectionFixtures;

[CollectionDefinition(nameof(DeleteVehiclesBackgroundServiceCollection))]
public class DeleteVehiclesBackgroundServiceCollection :
    ICollectionFixture<DeleteVehiclesBackgroundServiceFixture>
{

}

public class DeleteVehiclesBackgroundServiceFixture : ConsumerFixture<DeleteBackgroundService<DeleteVehiclesCommand, Command>, Command, DeleteVehiclesCommand>
{

}


