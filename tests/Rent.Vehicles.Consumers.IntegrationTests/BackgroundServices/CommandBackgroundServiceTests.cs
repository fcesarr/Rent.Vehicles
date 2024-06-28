using AutoFixture;

using FluentAssertions;

using Microsoft.Extensions.Hosting;

using MongoDB.Driver;

using Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;
using Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures.RabbitMQ.CollectionFixtures;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Services.Interfaces;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices;

public abstract class CommandBackgroundServiceTests<TBackgroundService, TCommand, TEntity, TService> : IDisposable 
    where TBackgroundService : BackgroundService 
    where TCommand : Messages.Command
    where TEntity : Entities.Command
    where TService : IDataService<TEntity>
{
    protected readonly ConsumerFixture<TBackgroundService, TCommand, TEntity, TService> _fixture;

    private readonly ITestOutputHelper _output;

    public CommandBackgroundServiceTests(
        ConsumerFixture<TBackgroundService, TCommand, TEntity, TService> fixture,
        ITestOutputHelper output)
    {
        _fixture = fixture;
        
        _output = output;

        _fixture.Init(output);
    }

    protected abstract TCommand GetCommand();

    [Fact]
    public async Task SendCreateVehiclesCommandAndVerifyCommandIsCreatedInDatabase()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000));

        var command = GetCommand();

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
