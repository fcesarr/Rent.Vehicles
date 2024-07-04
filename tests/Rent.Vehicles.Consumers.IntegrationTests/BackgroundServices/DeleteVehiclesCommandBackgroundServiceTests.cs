using System.Linq.Expressions;
using System.Runtime.InteropServices.Marshalling;

using AutoFixture;

using FluentAssertions;

using Rent.Vehicles.Consumers.Commands.BackgroundServices;
using Rent.Vehicles.Consumers.Events.BackgroundServices;
using Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;
using Rent.Vehicles.Consumers.IntegrationTests.Extensions;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Exceptions;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices;

[Collection(nameof(CommonCollectionFixture))]
public class DeleteVehiclesCommandBackgroundServiceTests : CommandBackgroundServiceTests
{
    private readonly Fixture _fixture;

    public DeleteVehiclesCommandBackgroundServiceTests(CommonFixture classFixture) : base(classFixture)
    {
        _fixture = new Fixture();

        _queues.Add("DeleteVehiclesCommand");
        _queues.Add("DeleteVehiclesEvent");
        _queues.Add("DeleteVehiclesProjectionEvent");
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

    [Fact]
    public async Task SendDeleteVehiclesCommandVerifyEntityAndProjectionAreDeleted()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        var command = _fixture
            .Build<DeleteVehiclesCommand>()
            .Create();

        await _classFixture.GetRequiredService<IPublisher>()
            .PublishCommandAsync(command, cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<DeleteVehiclesCommandBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<DeleteVehiclesEventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<DeleteVehiclesProjectionEventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        var commandDataService = _classFixture
            .GetRequiredService<ICommandDataService>();

        var entityDataService = _classFixture
            .GetRequiredService<IVehicleDataService>();

        var projectionDataService = _classFixture
            .GetRequiredService<IVehicleProjectionDataService>();

        var entityRepository = _classFixture
            .GetRequiredService<IRepository<Vehicle>>();

        var @event = _fixture
            .Build<Vehicle>()
                .With(x => x.Id, command.Id)
                .With(x => x.IsRented, false)
            .Create(); 

        await entityRepository.CreateAsync(@event, cancellationTokenSource.Token);

        var found = false;

        do
        {
            var commandResult = await commandDataService
                .GetAsync(x => x.SagaId == command.SagaId);

            var entityResult = await entityDataService
                .GetAsync(GetPredicate(@event));

            found = commandResult.IsSuccess &&
                !entityResult.IsSuccess &&
                entityResult.Exception is not null &&
                entityResult.Exception.GetType() == typeof(NullException);

            await periodicTimer.WaitForNextTickAsync(cancellationTokenSource.Token);
        } while (!found && !cancellationTokenSource.IsCancellationRequested);

        // Assert
        found.Should().BeTrue();

        await _classFixture.GetRequiredService<DeleteVehiclesProjectionEventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<DeleteVehiclesEventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
        
        await _classFixture.GetRequiredService<DeleteVehiclesCommandBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
    }

    [Fact]
    public async Task SendDeleteVehiclesCommandVerifyEntityAndEventFail()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        var command = _fixture
            .Build<DeleteVehiclesCommand>()
            .Create();

        await _classFixture.GetRequiredService<IPublisher>()
            .PublishCommandAsync(command, cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<DeleteVehiclesCommandBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<DeleteVehiclesEventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<EventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        var commandDataService = _classFixture
            .GetRequiredService<ICommandDataService>();

        var eventDataService = _classFixture
            .GetRequiredService<IEventDataService>();

        var vehicleRepository = _classFixture
            .GetRequiredService<IRepository<Vehicle>>();

        var entity = _fixture
            .Build<Vehicle>()
                .With(x => x.Id, command.Id)
                .With(x => x.IsRented, true)
            .Create(); 

        await vehicleRepository.CreateAsync(entity, cancellationTokenSource.Token);

        var found = false;

        do
        {
            var commandResult = await commandDataService
                .GetAsync(x => x.SagaId == command.SagaId);

            var eventResult = await eventDataService
                .GetAsync(x => x.SagaId == command.SagaId && 
                    x.StatusType == Entities.Types.StatusType.Fail &&
                    x.Name == typeof(DeleteVehiclesEvent).Name);

            found = commandResult.IsSuccess &&
                eventResult.IsSuccess;

            await periodicTimer.WaitForNextTickAsync(cancellationTokenSource.Token);
        } while (!found && !cancellationTokenSource.IsCancellationRequested);

        // Assert
        found.Should().BeTrue();

        await _classFixture.GetRequiredService<EventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<DeleteVehiclesEventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
        
        await _classFixture.GetRequiredService<DeleteVehiclesCommandBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
    }
}
