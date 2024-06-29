
using System.Linq.Expressions;

using AutoFixture;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Rent.Vehicles.Consumers.Commands.BackgroundServices;
using Rent.Vehicles.Consumers.Events.BackgroundServices;
using Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;
using Rent.Vehicles.Consumers.IntegrationTests.Extensions;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices;

[Collection(nameof(CommonCollection))]
public class CreateVehiclesCommandBackgroundServiceTests
{
    private readonly Fixture _fixture;

    private readonly CommonFixture _classFixture;

    public CreateVehiclesCommandBackgroundServiceTests(CommonFixture classFixture)
    {
        _fixture = new Fixture();
        _classFixture = classFixture;
    }

    private Expression<Func<TEntity, bool>> GetPredicate<TEntity>(Guid id) where TEntity : Entity => x => x.Id == id;

    private Expression<Func<Vehicle, bool>> GetVehiclePredicate(CreateVehiclesCommand command)
    {
        var predicate = GetPredicate<Vehicle>(command.Id);

        return predicate.And(y => y.Year == command.Year &&
            y.LicensePlate == command.LicensePlate &&
            y.Type == (Entities.Types.VehicleType)command.Type &&
            y.Model == command.Model);
    }

    private Expression<Func<TProjection, bool>> GetVehicleProjectionPredicate<TProjection>(CreateVehiclesCommand command) where TProjection : VehicleProjection
    {
        var predicate = GetPredicate<TProjection>(command.Id);

        return predicate.And(y => y.Year == command.Year &&
            y.LicensePlate == command.LicensePlate &&
            y.Type == (Entities.Types.VehicleType)command.Type &&
            y.Model == command.Model);
    }

    [Fact]
    public async Task SendCommandAndVerifyCommandIsCreatedInDatabase()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        var command = _fixture
            .Build<CreateVehiclesCommand>()
            .With(x => x.Year, () => {
                var random = new Random();
                return random.Next(2024, 2024);
            })
            .Create();

        await _classFixture.GetRequiredService<IPublisher>()
            .PublishCommandAsync(command, cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateVehiclesCommandBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateVehiclesEventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateVehiclesProjectionEventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateVehiclesForSpecificYearEventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateVehiclesForSpecificYearProjectionEventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        var commandDataService = _classFixture
            .GetRequiredService<ICommandDataService>();

        var vehicleDataService = _classFixture
            .GetRequiredService<IVehicleDataService>();

        var vehicleProjectionDataService = _classFixture
            .GetRequiredService<IVehicleProjectionDataService>();

        var vehiclesForSpecificYearProjectionDataService = _classFixture
            .GetRequiredService<IVehiclesForSpecificYearProjectionDataService>();

        var found = false;

        do
        {
            var commandResult = await commandDataService
                .GetAsync(x => x.SagaId == command.SagaId);

            var vehicleResult = await vehicleDataService
                .GetAsync(GetVehiclePredicate(command));

            var vehicleProjectionResult = await vehicleProjectionDataService
                .GetAsync(GetVehicleProjectionPredicate<VehicleProjection>(command));

            var vehiclesForSpecificYearProjectionResult = await vehiclesForSpecificYearProjectionDataService
                .GetAsync(GetVehicleProjectionPredicate<VehiclesForSpecificYearProjection>(command));

            found = commandResult.IsSuccess && 
                vehicleProjectionResult.IsSuccess && 
                vehicleProjectionResult.IsSuccess &&
                vehiclesForSpecificYearProjectionResult.IsSuccess;

            await periodicTimer.WaitForNextTickAsync(cancellationTokenSource.Token);
        } while (!found && !cancellationTokenSource.IsCancellationRequested);

        // Assert
        found.Should().BeTrue();

        await _classFixture.GetRequiredService<CreateVehiclesForSpecificYearProjectionEventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateVehiclesForSpecificYearEventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateVehiclesProjectionEventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateVehiclesEventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
        
        await _classFixture.GetRequiredService<CreateVehiclesCommandBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
    }

    [Fact]
    public async Task SenddCommandAndVerifyCommandIsCreatedInDatabase()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(90));

        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        var command = _fixture
            .Build<CreateVehiclesCommand>()
            .With(x => x.Year, 2025)
            .Create();

        await _classFixture.GetRequiredService<IPublisher>()
            .PublishCommandAsync(command, cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateVehiclesCommandBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateVehiclesEventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateVehiclesProjectionEventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateVehiclesForSpecificYearEventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateVehiclesForSpecificYearProjectionEventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        var commandDataService = _classFixture
            .GetRequiredService<ICommandDataService>();

        var vehicleDataService = _classFixture
            .GetRequiredService<IVehicleDataService>();

        var vehicleProjectionDataService = _classFixture
            .GetRequiredService<IVehicleProjectionDataService>();

        var vehiclesForSpecificYearProjectionDataService = _classFixture
            .GetRequiredService<IVehiclesForSpecificYearProjectionDataService>();

        var found = false;

        do
        {
            var commandResult = await commandDataService
                .GetAsync(x => x.SagaId == command.SagaId);

            var vehicleResult = await vehicleDataService
                .GetAsync(GetVehiclePredicate(command));

            var vehicleProjectionResult = await vehicleProjectionDataService
                .GetAsync(GetVehicleProjectionPredicate<VehicleProjection>(command));

            var vehiclesForSpecificYearProjectionResult = await vehiclesForSpecificYearProjectionDataService
                .GetAsync(GetVehicleProjectionPredicate<VehiclesForSpecificYearProjection>(command));

            found = commandResult.IsSuccess && 
                vehicleProjectionResult.IsSuccess && 
                vehicleProjectionResult.IsSuccess &&
                !vehiclesForSpecificYearProjectionResult.IsSuccess &&
                vehiclesForSpecificYearProjectionResult.Exception is not null;

            await periodicTimer.WaitForNextTickAsync(cancellationTokenSource.Token);
        } while (!found && !cancellationTokenSource.IsCancellationRequested);

        // Assert
        found.Should().BeTrue();

        await _classFixture.GetRequiredService<CreateVehiclesForSpecificYearProjectionEventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateVehiclesForSpecificYearEventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateVehiclesProjectionEventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateVehiclesEventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
        
        await _classFixture.GetRequiredService<CreateVehiclesCommandBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
    }
}
