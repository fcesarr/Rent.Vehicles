using System.Windows.Input;

using Rent.Vehicles.Consumers.Commands.BackgroundServices;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Services.DataServices.Interfaces;


namespace Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures.RabbitMQ.CollectionFixtures;

[CollectionDefinition(nameof(UpdateVehiclesCommandBackgroundServiceCollection))]
public class UpdateVehiclesCommandBackgroundServiceCollection :
    ICollectionFixture<UpdateVehiclesCommandBackgroundServiceFixture>
{

}

public class UpdateVehiclesCommandBackgroundServiceFixture : ConsumerFixture<UpdateVehiclesCommandBackgroundService, UpdateVehiclesCommand, Command, ICommandDataService>
{

}


