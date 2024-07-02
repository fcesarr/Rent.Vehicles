
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
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Extensions;

using Xunit.Abstractions;
using RabbitMQ.Client;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices;

[Collection(nameof(CommonCollectionFixture))]
public class CreateVehiclesCommandBackgroundServiceTests : CommandBackgroundServiceTests
{
    private readonly Fixture _fixture;

    public CreateVehiclesCommandBackgroundServiceTests(CommonFixture classFixture) : base(classFixture)
    {
        _fixture = new Fixture();

        _queues.Add("CreateVehiclesCommand");
        _queues.Add("CreateVehiclesEvent");
        _queues.Add("CreateVehiclesForSpecificYearEvent");
        _queues.Add("CreateVehiclesForSpecificYearProjectionEvent");
        _queues.Add("CreateVehiclesProjectionEvent");
        _queues.Add("Event");
    }

    private static Expression<Func<TEntity, bool>> GetPredicate<TEntity>(Guid id) where TEntity : Entity => x => x.Id == id;

    private static Expression<Func<Vehicle, bool>> GetPredicate(CreateVehiclesCommand command)
    {
        var predicate = GetPredicate<Vehicle>(command.Id);

        return predicate.And(x => x.Year == command.Year &&
            x.LicensePlate == command.LicensePlate &&
            x.Type == (Entities.Types.VehicleType)command.Type &&
            x.Model == command.Model);
    }

    private static Expression<Func<TProjection, bool>> GetProjectionPredicate<TProjection>(CreateVehiclesCommand command) where TProjection : VehicleProjection
    {
        var predicate = GetPredicate<TProjection>(command.Id);

        return predicate.And(x => x.Year == command.Year &&
            x.LicensePlate == command.LicensePlate &&
            x.Type == (Entities.Types.VehicleType)command.Type &&
            x.Model == command.Model);
    }

    [Fact]
    public async Task SendCreateVehiclesCommandWithYearEqual2024VerifyEntityAndProjectionAreSaved()
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

        var entityDataService = _classFixture
            .GetRequiredService<IVehicleDataService>();

        var projectionDataService = _classFixture
            .GetRequiredService<IVehicleProjectionDataService>();

        var yearProjectionDataService = _classFixture
            .GetRequiredService<IVehiclesForSpecificYearProjectionDataService>();

        var found = false;

        do
        {
            var commandResult = await commandDataService
                .GetAsync(x => x.SagaId == command.SagaId);

            var entityResult = await entityDataService
                .GetAsync(GetPredicate(command));

            var projectionResult = await projectionDataService
                .GetAsync(GetProjectionPredicate<VehicleProjection>(command));

            var yearProjectionResult = await yearProjectionDataService
                .GetAsync(GetProjectionPredicate<VehiclesForSpecificYearProjection>(command));

            found = commandResult.IsSuccess &&
                entityResult.IsSuccess &&
                projectionResult.IsSuccess && 
                projectionResult.IsSuccess &&
                yearProjectionResult.IsSuccess;

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
    public async Task SendCreateVehiclesCommandWithYearDiff2024VerifyEntityAndProjectionAreSaved()
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

        var entityDataService = _classFixture
            .GetRequiredService<IVehicleDataService>();

        var projectionDataService = _classFixture
            .GetRequiredService<IVehicleProjectionDataService>();

        var yearProjectionDataService = _classFixture
            .GetRequiredService<IVehiclesForSpecificYearProjectionDataService>();

        var found = false;

        do
        {
            var commandResult = await commandDataService
                .GetAsync(x => x.SagaId == command.SagaId);

            var entityResult = await entityDataService
                .GetAsync(GetPredicate(command));

            var projectionResult = await projectionDataService
                .GetAsync(GetProjectionPredicate<VehicleProjection>(command));

            var yearProjectionResult = await yearProjectionDataService
                .GetAsync(GetProjectionPredicate<VehiclesForSpecificYearProjection>(command));

            found = commandResult.IsSuccess &&
                entityResult.IsSuccess &&
                projectionResult.IsSuccess && 
                projectionResult.IsSuccess &&
                !yearProjectionResult.IsSuccess &&
                yearProjectionResult.Exception is not null;

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
    public async Task SendCreateVehiclesCommandWithSameLicensePlateVerifyEntityAndEventFail()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(90));

        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        var entityRepository = _classFixture
            .GetRequiredService<IRepository<Vehicle>>();

        var entity = _fixture
            .Build<Vehicle>()
            .Create(); 

        await entityRepository.CreateAsync(entity, cancellationTokenSource.Token);

        var command = _fixture
            .Build<CreateVehiclesCommand>()
                .With(x => x.LicensePlate, entity.LicensePlate)
            .Create();
        
        await _classFixture.GetRequiredService<IPublisher>()
            .PublishCommandAsync(command, cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateVehiclesCommandBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateVehiclesEventBackgroundService>()
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
                    x.Name == typeof(CreateVehiclesEvent).Name);

            found = commandResult.IsSuccess &&
                eventResult.IsSuccess;

            await periodicTimer.WaitForNextTickAsync(cancellationTokenSource.Token);
        } while (!found && !cancellationTokenSource.IsCancellationRequested);

        // Assert
        found.Should().BeTrue();

        await _classFixture.GetRequiredService<EventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateVehiclesEventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
        
        await _classFixture.GetRequiredService<CreateVehiclesCommandBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
    }
}
