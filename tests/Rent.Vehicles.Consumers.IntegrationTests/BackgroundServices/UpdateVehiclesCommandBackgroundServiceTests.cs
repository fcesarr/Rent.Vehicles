using System.Linq.Expressions;

using AutoFixture;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using Rent.Vehicles.Consumers.Commands.BackgroundServices;
using Rent.Vehicles.Consumers.Events.BackgroundServices;
using Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;
using Rent.Vehicles.Consumers.IntegrationTests.Extensions;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Contexts.Interfaces;
using Rent.Vehicles.Entities.Factories.Interfaces;
using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices;

[Collection(nameof(CommonCollectionFixture))]
public class UpdateVehiclesCommandBackgroundServiceTests : CommandBackgroundServiceTests
{
    private readonly Fixture _fixture;

    public UpdateVehiclesCommandBackgroundServiceTests(CommonFixture classFixture) : base(classFixture)
    {
        _fixture = new Fixture();

        _queues.Add("UpdateVehiclesCommand");
        _queues.Add("UpdateVehiclesEvent");
        _queues.Add("UpdateVehiclesProjectionEvent");
        _queues.Add("Event");
    }

    private static Expression<Func<TEntity, bool>> GetPredicate<TEntity>(Guid id) where TEntity : Entity => x => x.Id == id;

    private static Expression<Func<Vehicle, bool>> GetPredicate(Vehicle entity)
    {
        var predicate = GetPredicate<Vehicle>(entity.Id);

        return predicate.And(x => x.Year == entity.Year &&
            x.LicensePlate == entity.LicensePlate &&
            x.Type == (Entities.Types.VehicleType)entity.Type &&
            x.Model == entity.Model);
    }

    private static Expression<Func<VehicleProjection, bool>> GetProjectionPredicate(Vehicle entity)
    {
        var predicate = GetPredicate<VehicleProjection>(entity.Id);

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

        var vehicleRepository = _classFixture
            .GetRequiredService<IRepository<Vehicle>>();

        var entity = _fixture
            .Build<Vehicle>()
                .With(x => x.IsRented, false)
            .Create(); 

        await vehicleRepository.CreateAsync(entity, cancellationTokenSource.Token);

        var command = _fixture
            .Build<UpdateVehiclesCommand>()
                .With(x => x.Id, entity.Id)
            .Create();

        entity.LicensePlate = command.LicensePlate;

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

        var entityDataService = _classFixture
            .GetRequiredService<IVehicleDataService>();

        var projectionDataService = _classFixture
            .GetRequiredService<IVehicleProjectionDataService>();

        var found = false;

        do
        {
            var commandResult = await commandDataService
                .GetAsync(x => x.SagaId == command.SagaId);

            var entityResult = await entityDataService
                .GetAsync(GetPredicate(entity));

            var projectionResult = await projectionDataService
                .GetAsync(GetProjectionPredicate(entity));

            found = commandResult.IsSuccess &&
                entityResult.IsSuccess &&
                entityResult.Value!.LicensePlate == command.LicensePlate &&
                projectionResult.IsSuccess &&
                projectionResult.Value!.LicensePlate == command.LicensePlate;

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

    [Fact]
    public async Task SendUpdateVehiclesCommandVerifyEntityAndEventFail()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        var entityRepository = _classFixture
            .GetRequiredService<IRepository<Vehicle>>();

        var entity = _fixture
            .Build<Vehicle>()
                .With(x => x.IsRented, false)
            .Create(); 

        await entityRepository.CreateAsync(entity, cancellationTokenSource.Token);

        var licensePlate = entity.LicensePlate;

        entity = _fixture
            .Build<Vehicle>()
                .With(x => x.IsRented, false)
            .Create(); 

        await entityRepository.CreateAsync(entity, cancellationTokenSource.Token);

        var command = _fixture
            .Build<UpdateVehiclesCommand>()
                .With(x => x.Id, entity.Id)
                .With(x => x.LicensePlate, licensePlate)
            .Create();

        await _classFixture.GetRequiredService<IPublisher>()
            .PublishCommandAsync(command, cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UpdateVehiclesCommandBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UpdateVehiclesEventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<EventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        var commandDataService = _classFixture
            .GetRequiredService<ICommandDataService>();

        var eventDataService = _classFixture
            .GetRequiredService<IEventDataService>();

        var found = false;

        do
        {
            var commandResult = await commandDataService
                .GetAsync(x => x.SagaId == command.SagaId);

            var eventResult = await eventDataService
                .GetAsync(x => x.SagaId == command.SagaId && 
                    x.StatusType == Entities.Types.StatusType.Fail &&
                    x.Message.Contains("Error on Validate") &&
                    x.Name == typeof(UpdateVehiclesEvent).Name);

            found = commandResult.IsSuccess &&
                eventResult.IsSuccess;

            await periodicTimer.WaitForNextTickAsync(cancellationTokenSource.Token);
        } while (!found && !cancellationTokenSource.IsCancellationRequested);

        // Assert
        found.Should().BeTrue();

        await _classFixture.GetRequiredService<EventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UpdateVehiclesEventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
        
        await _classFixture.GetRequiredService<UpdateVehiclesCommandBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
    }
}
