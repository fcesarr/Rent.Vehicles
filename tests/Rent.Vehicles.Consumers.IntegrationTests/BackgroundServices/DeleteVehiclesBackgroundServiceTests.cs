using AutoFixture;

using FluentAssertions;

using Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures.RabbitMQ.CollectionFixtures;
using Rent.Vehicles.Messages.Commands;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices;

[Collection(nameof(DeleteVehiclesBackgroundServiceCollection))]
public class DeleteVehiclesBackgroundServiceTests : IDisposable
{
    private readonly DeleteVehiclesBackgroundServiceFixture _fixture;
    private readonly ITestOutputHelper _output;

    public DeleteVehiclesBackgroundServiceTests(
        DeleteVehiclesBackgroundServiceFixture fixture,
        ITestOutputHelper output)
    {
        _fixture = fixture;
        
        _output = output;

        _fixture.Init(output);
    }

    [Fact]
    public async Task SendDeleteVehiclesCommandAndVerifyCommandIsCreatedInDatabase()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000));

        var command = _fixture.GetFixture()
            .Create<DeleteVehiclesCommand>();

        await _fixture.SendCommandAsync(command, cancellationTokenSource.Token);

        await _fixture.StartWorkerAsync(cancellationTokenSource.Token);

        var found = false;

        do
        {
            found = await _fixture.GetCommandAsync(x => x.SagaId == command.SagaId) != null;
            await periodicTimer.WaitForNextTickAsync(cancellationTokenSource.Token);
        } while (!found && !cancellationTokenSource.IsCancellationRequested);

        // Assert
        found.Should().BeTrue();

        await _fixture.StopWorkerAsync(cancellationTokenSource.Token);
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}