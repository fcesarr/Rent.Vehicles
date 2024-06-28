using System.Windows.Input;

using Rent.Vehicles.Consumers.Commands.BackgroundServices;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Services.DataServices.Interfaces;


namespace Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures.RabbitMQ.CollectionFixtures;

[CollectionDefinition(nameof(CreateVehiclesCommandBackgroundServiceCollection))]
public class CreateVehiclesCommandBackgroundServiceCollection :
    ICollectionFixture<CreateVehiclesCommandBackgroundServiceFixture>
{

}

public class CreateVehiclesCommandBackgroundServiceFixture : ConsumerFixture<CreateVehiclesCommandBackgroundService, CreateVehiclesCommand, Command, ICommandDataService>
{

}


