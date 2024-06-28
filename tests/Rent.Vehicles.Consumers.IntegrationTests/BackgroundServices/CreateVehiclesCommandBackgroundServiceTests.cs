using AutoFixture;

using FluentAssertions;

using Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures.RabbitMQ.CollectionFixtures;
using Rent.Vehicles.Messages.Commands;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices;

[Collection(nameof(CreateVehiclesCommandBackgroundServiceCollection))]
public class CreateVehiclesCommandBackgroundServiceTests : IDisposable
{
    private readonly CreateVehiclesCommandBackgroundServiceFixture _fixture;
    private readonly ITestOutputHelper _output;

    public CreateVehiclesCommandBackgroundServiceTests(
        CreateVehiclesCommandBackgroundServiceFixture fixture,
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

        var command = _fixture
            .GetFixture()
            .Build<CreateVehiclesCommand>()
            .With(x => x.Year, () => {
                var random = new Random();
                return random.Next(2024, 2024);
            })
            .Create();

        await _fixture.SendCommandAsync(command, cancellationTokenSource.Token);

        await _fixture.StartWorkerAsync(cancellationTokenSource.Token);

        var found = false;

        do
        {
            var result = await _fixture.GetCommandAsync(x => x.SagaId == command.SagaId);

            found = result!.IsSuccess;

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
