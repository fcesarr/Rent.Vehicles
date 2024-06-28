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

[Collection(nameof(CreateVehiclesCommandBackgroundServiceCollection))]
public class CreateVehiclesCommandBackgroundServiceTests : CommandBackgroundServiceTests<CreateVehiclesCommandBackgroundService, CreateVehiclesCommand, Entities.Command, ICommandDataService>
{
    public CreateVehiclesCommandBackgroundServiceTests(CreateVehiclesCommandBackgroundServiceFixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
    }

    protected override CreateVehiclesCommand GetCommand()
    {
        return _fixture
            .GetFixture()
            .Build<CreateVehiclesCommand>()
            .With(x => x.Year, () => {
                var random = new Random();
                return random.Next(2024, 2024);
            })
            .Create();
    }
}
