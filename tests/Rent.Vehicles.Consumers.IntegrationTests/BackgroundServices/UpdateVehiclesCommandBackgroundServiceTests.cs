using System.Linq.Expressions;

using AutoFixture;

using FluentAssertions;

using Rent.Vehicles.Consumers.Commands.BackgroundServices;
using Rent.Vehicles.Consumers.Events.BackgroundServices;
using Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;
using Rent.Vehicles.Consumers.IntegrationTests.Extensions;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices;

[Collection(nameof(CommonCollection))]
public class UpdateVehiclesCommandBackgroundServiceTests
{
    private readonly Fixture _fixture;

    private readonly CommonFixture _classFixture;

    public UpdateVehiclesCommandBackgroundServiceTests(CommonFixture classFixture)
    {
        _fixture = new Fixture();
        _classFixture = classFixture;
    }

    private static Expression<Func<TEntity, bool>> GetPredicate<TEntity>(Guid id) where TEntity : Entity => x => x.Id == id;

    private static Expression<Func<Vehicle, bool>> GetVehiclePredicate(Vehicle entity)
    {
        var predicate = GetPredicate<Vehicle>(entity.Id);

        return predicate.And(x => x.Year == entity.Year &&
            x.LicensePlate == entity.LicensePlate &&
            x.Type == (Entities.Types.VehicleType)entity.Type &&
            x.Model == entity.Model);
    }

    [Fact]
    public async Task SendUpdateVehiclesCommandVerifyEntityAndProjectionAreUpdated()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        var command = _fixture
            .Build<UpdateVehiclesCommand>()
            .Create();

        await _classFixture.GetRequiredService<IPublisher>()
            .PublishCommandAsync(command, cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UpdateVehiclesCommandBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UpdateVehiclesEventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UpdateVehiclesProjectionEventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        var commandDataService = _classFixture
            .GetRequiredService<ICommandDataService>();

        var vehicleDataService = _classFixture
            .GetRequiredService<IVehicleDataService>();

        var vehicleProjectionDataService = _classFixture
            .GetRequiredService<IVehicleProjectionDataService>();

        var vehicleRepository = _classFixture
            .GetRequiredService<IRepository<Vehicle>>();

        var @event = _fixture
            .Build<Vehicle>()
                .With(x => x.Id, command.Id)
                .With(x => x.IsRented, false)
            .Create(); 

        await vehicleRepository.CreateAsync(@event, cancellationTokenSource.Token);

        var found = false;

        do
        {
            var commandResult = await commandDataService
                .GetAsync(x => x.SagaId == command.SagaId);

            @event.LicensePlate = command.LicensePlate;
            var vehicleResult = await vehicleDataService
                .GetAsync(GetVehiclePredicate(@event));

            found = commandResult.IsSuccess &&
                vehicleResult.IsSuccess &&
                vehicleResult.Value!.LicensePlate == command.LicensePlate;

            await periodicTimer.WaitForNextTickAsync(cancellationTokenSource.Token);
        } while (!found && !cancellationTokenSource.IsCancellationRequested);

        // Assert
        found.Should().BeTrue();

        await _classFixture.GetRequiredService<UpdateVehiclesProjectionEventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UpdateVehiclesEventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
        
        await _classFixture.GetRequiredService<UpdateVehiclesCommandBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
    }
}
