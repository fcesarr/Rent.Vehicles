using AutoFixture;

using FluentAssertions;

using Rent.Vehicles.Messages;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices;

[Collection(nameof(CreateVehiclesBackgroundServiceCollection))]
public class CreateVehiclesBackgroundServiceTests : IDisposable
{
    private readonly CreateVehiclesBackgroundServiceFixture _fixture;
    private readonly ITestOutputHelper _output;

    public CreateVehiclesBackgroundServiceTests(
        CreateVehiclesBackgroundServiceFixture fixture,
        ITestOutputHelper output)
    {
        _fixture = fixture;
        
        _output = output;

        _fixture.Init(output);
    }

    [Fact]
    public async Task SendCreateCommand_EmptyQueue_Success()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));

        var command = _fixture.GetFixture()
            .Create<CreateVehiclesCommand>();

        await _fixture.SendCommandAsync(command, cancellationTokenSource.Token);

        await _fixture.StartWorkerAsync(cancellationTokenSource.Token);

        var found = false;

        do
        {
            found = _fixture.QueueCount("CreateVehiclesCommand") == 0;
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