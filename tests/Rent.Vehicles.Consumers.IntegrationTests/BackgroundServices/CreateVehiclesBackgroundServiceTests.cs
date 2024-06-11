using AutoFixture;

using FluentAssertions;

using Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures.RabbitMQ.CollectionFixtures;
using Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices;
using Rent.Vehicles.Messages.Commands;

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
    public async Task SendCreateVehiclesCommandAndVerifyCommandIsCreatedInDatabase()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000));

        var command = _fixture.GetFixture()
            .Create<CreateVehiclesCommand>();

        await _fixture.SendCommandAsync(command, cancellationTokenSource.Token);

        await _fixture.StartWorkerAsync(cancellationTokenSource.Token);

        // await _fixture.StartWorkerEventAsync<CreateVehiclesEventBackgroundService>(cancellationTokenSource.Token);

        // await _fixture.StartWorkerEventAsync<CreateVehiclesYearEventBackgroundService>(cancellationTokenSource.Token);

        var found = false;

        do
        {
            found = await _fixture.GetCommandAsync(@"SELECT * FROM Commands WHERE SagaId = @SagaId",
                new Dictionary<string, dynamic>{
                    { "@SagaId", command.SagaId }
                }
            ) != null;
            await periodicTimer.WaitForNextTickAsync(cancellationTokenSource.Token);
        } while (!found && !cancellationTokenSource.IsCancellationRequested);

        // Assert
        found.Should().BeTrue();

        // await _fixture.StopWorkerEventAsync<CreateVehiclesYearEventBackgroundService>(cancellationTokenSource.Token);

        // await _fixture.StopWorkerEventAsync<CreateVehiclesEventBackgroundService>(cancellationTokenSource.Token);

        await _fixture.StopWorkerAsync(cancellationTokenSource.Token);
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}