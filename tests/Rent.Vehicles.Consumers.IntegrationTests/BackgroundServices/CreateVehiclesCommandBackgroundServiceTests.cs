
using AutoFixture;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Rent.Vehicles.Consumers.Commands.BackgroundServices;
using Rent.Vehicles.Consumers.Events.BackgroundServices;
using Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.DataServices.Interfaces;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices;

[Collection(nameof(CommonCollection))]
public class CreateVehiclesCommandBackgroundServiceTests : CommandBackgroundServiceTests<CreateVehiclesCommandBackgroundService, CreateVehiclesCommand>
{
    private readonly Fixture _fixture;

    public CreateVehiclesCommandBackgroundServiceTests(CommonFixture classFixture, ITestOutputHelper output) : base(classFixture, output)
    {
        _fixture = new Fixture();
    }

    protected override CreateVehiclesCommand GetCommand()
    {
        return _fixture
            .Build<CreateVehiclesCommand>()
            .With(x => x.Year, () => {
                var random = new Random();
                return random.Next(2024, 2024);
            })
            .Create();
    }
}
