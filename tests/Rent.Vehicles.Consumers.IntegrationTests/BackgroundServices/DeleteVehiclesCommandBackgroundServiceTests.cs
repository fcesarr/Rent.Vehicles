using AutoFixture;

using FluentAssertions;

using Rent.Vehicles.Consumers.Commands.BackgroundServices;
using Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Services.DataServices.Interfaces;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices;

[Collection(nameof(CommonCollection))]
public class DeleteVehiclesCommandBackgroundServiceTests : CommandBackgroundServiceTests<DeleteVehiclesCommandBackgroundService, DeleteVehiclesCommand>
{
    private readonly Fixture _fixture;

    public DeleteVehiclesCommandBackgroundServiceTests(CommonFixture classFixture, ITestOutputHelper output) : base(classFixture, output)
    {
        _fixture = new Fixture();
    }

    protected override DeleteVehiclesCommand GetCommand()
    {
        return _fixture
            .Build<DeleteVehiclesCommand>()
            .Create();
    }
}
