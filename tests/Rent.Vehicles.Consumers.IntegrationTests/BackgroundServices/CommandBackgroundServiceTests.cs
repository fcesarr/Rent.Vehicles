using AutoFixture;

using FluentAssertions;

using Microsoft.Extensions.Hosting;

using MongoDB.Driver;

using Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Interfaces;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices;

public abstract class CommandBackgroundServiceTests<TBackgroundService, TCommand> 
    where TBackgroundService : BackgroundService
    where TCommand : Command
{
    protected readonly CommonFixture _classFixture;

    public CommandBackgroundServiceTests(
        CommonFixture classFixture,
        ITestOutputHelper output)
    {
        _classFixture = classFixture;
    }

    protected abstract TCommand GetCommand();

    [Fact]
    public async Task SendCommandAndVerifyCommandIsCreatedInDatabase()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000));

        var command = GetCommand();

        await _classFixture.GetRequiredService<IPublisher>()
            .PublishCommandAsync(command, cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<TBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        var found = false;

        do
        {
            var result = await _classFixture.GetRequiredService<ICommandDataService>()
                .GetAsync(x => x.SagaId == command.SagaId);

            found = result!.IsSuccess;

            await periodicTimer.WaitForNextTickAsync(cancellationTokenSource.Token);
        } while (!found && !cancellationTokenSource.IsCancellationRequested);

        // Assert
        found.Should().BeTrue();

        await _classFixture.GetRequiredService<TBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
    }
}
