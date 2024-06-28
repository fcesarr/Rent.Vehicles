using AutoFixture;

using FluentAssertions;

using Rent.Vehicles.Consumers.Commands.BackgroundServices;
using Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;
using Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures.RabbitMQ.CollectionFixtures;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Services.DataServices.Interfaces;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices;

[Collection(nameof(DeleteVehiclesCommandBackgroundServiceCollection))]
public class DeleteVehiclesCommandBackgroundServiceTests : CommandBackgroundServiceTests<DeleteVehiclesCommandBackgroundService, DeleteVehiclesCommand, Entities.Command, ICommandDataService>
{
    public DeleteVehiclesCommandBackgroundServiceTests(DeleteVehiclesCommandBackgroundServiceFixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
    }

    protected override DeleteVehiclesCommand GetCommand()
    {
        return _fixture
            .GetFixture()
            .Build<DeleteVehiclesCommand>()
            .Create();
    }
}
