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

[Collection(nameof(UpdateVehiclesCommandBackgroundServiceCollection))]
public class UpdateVehiclesCommandBackgroundServiceTests : CommandBackgroundServiceTests<UpdateVehiclesCommandBackgroundService, UpdateVehiclesCommand, Entities.Command, ICommandDataService>
{
    public UpdateVehiclesCommandBackgroundServiceTests(UpdateVehiclesCommandBackgroundServiceFixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
    }

    protected override UpdateVehiclesCommand GetCommand()
    {
        return _fixture
            .GetFixture()
            .Build<UpdateVehiclesCommand>()
            .Create();
    }
}
