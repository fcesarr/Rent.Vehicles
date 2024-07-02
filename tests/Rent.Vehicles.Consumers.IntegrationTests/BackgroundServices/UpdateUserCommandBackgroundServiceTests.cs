
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
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices;

[Collection(nameof(CommonCollectionFixture))]
public class UpdateUserCommandBackgroundServiceTests : CommandBackgroundServiceTests
{
    private readonly Fixture _fixture;

    public UpdateUserCommandBackgroundServiceTests(CommonFixture classFixture) : base(classFixture)
    {
        _fixture = new Fixture();

        _queues.Add("UpdateUserCommand");
        _queues.Add("UpdateUserEvent");
        _queues.Add("UpdateUserProjectionEvent");
        _queues.Add("Event");
    }

    private static Expression<Func<TEntity, bool>> GetPredicate<TEntity>(Guid id) where TEntity : Entity => x => x.Id == id;

    private static Expression<Func<User, bool>> GetPredicate(UpdateUserCommand command)
    {
        var predicate = GetPredicate<User>(command.Id);

        return predicate.And(x => x.Name == command.Name &&
            x.Number == command.Number &&
            x.Birthday == command.Birthday &&
            x.LicenseNumber == command.LicenseNumber &&
            x.LicenseType == (Entities.Types.LicenseType)command.LicenseType);
    }

    private static Expression<Func<UserProjection, bool>> GetProjectionPredicate(UpdateUserCommand command)
    {
        var predicate = GetPredicate<UserProjection>(command.Id);

        return predicate.And(x => x.Name == command.Name &&
            x.Number == command.Number &&
            x.Birthday == command.Birthday &&
            x.LicenseNumber == command.LicenseNumber &&
            x.LicenseType == (Entities.Types.LicenseType)command.LicenseType);
    }

    [Fact]
    public async Task SendUpdateUserCommandVerifyEntityAndProjectionAreUpdated()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        var userRepository = _classFixture
            .GetRequiredService<IRepository<User>>();

        var entity = _fixture
            .Build<User>()
            .Create(); 

        await userRepository.CreateAsync(entity, cancellationTokenSource.Token);

        var command = _fixture
            .Build<UpdateUserCommand>()
                .With(x => x.Id, entity.Id)
            .Create();


        entity.Name = command.Name;
        entity.Number = command.Number;
        entity.Birthday = command.Birthday;
        entity.LicenseNumber = command.LicenseNumber;
        entity.LicenseType = (Entities.Types.LicenseType) command.LicenseType;

        await _classFixture.GetRequiredService<IPublisher>()
            .PublishCommandAsync(command, cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UpdateUserCommandBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UpdateUserEventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UpdateUserProjectionEventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        var commandDataService = _classFixture
            .GetRequiredService<ICommandDataService>();

        var entityDataService = _classFixture
            .GetRequiredService<IUserDataService>();

        var projectionDataService = _classFixture
            .GetRequiredService<IUserProjectionDataService>();

        var found = false;

        do
        {
            var commandResult = await commandDataService
                .GetAsync(x => x.SagaId == command.SagaId);

            var entityResult = await entityDataService
                .GetAsync(GetPredicate(command));

            var projectionResult = await projectionDataService
                .GetAsync(GetProjectionPredicate(command));

            found = commandResult.IsSuccess &&
                entityResult.IsSuccess &&
                entityResult.Value!.Name == command.Name &&
                entityResult.Value!.Number == command.Number &&
                entityResult.Value!.Birthday.Date == command.Birthday.Date &&
                entityResult.Value!.LicenseNumber == command.LicenseNumber &&
                entityResult.Value!.LicenseType == (Entities.Types.LicenseType) command.LicenseType &&
                projectionResult.IsSuccess &&
                projectionResult.Value!.Name == command.Name &&
                projectionResult.Value!.Number == command.Number &&
                projectionResult.Value!.Birthday.Date == command.Birthday.Date &&
                projectionResult.Value!.LicenseNumber == command.LicenseNumber &&
                projectionResult.Value!.LicenseType == (Entities.Types.LicenseType) command.LicenseType;

            await periodicTimer.WaitForNextTickAsync(cancellationTokenSource.Token);
        } while (!found && !cancellationTokenSource.IsCancellationRequested);

        // Assert
        found.Should().BeTrue();

        await _classFixture.GetRequiredService<UpdateUserProjectionEventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UpdateUserEventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
        
        await _classFixture.GetRequiredService<UpdateUserCommandBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
    }

    [Fact]
    public async Task SendUpdateUserCommandWithSameNumberVerifyEntityAndEventFail()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        var entityRepository = _classFixture
            .GetRequiredService<IRepository<User>>();

        var entity = _fixture
            .Build<User>()
            .Create(); 

        await entityRepository.CreateAsync(entity, cancellationTokenSource.Token);

        var number = entity.Number;

        entity = _fixture
            .Build<User>()
            .Create(); 

        await entityRepository.CreateAsync(entity, cancellationTokenSource.Token);

        var command = _fixture
            .Build<UpdateUserCommand>()
                .With(x => x.Id, entity.Id)
                .With(x => x.Number, number)
            .Create();

        await _classFixture.GetRequiredService<IPublisher>()
            .PublishCommandAsync(command, cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UpdateUserCommandBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UpdateUserEventBackgroundService>()
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
                    x.Name == typeof(UpdateUserEvent).Name);

            found = commandResult.IsSuccess &&
                eventResult.IsSuccess;

            await periodicTimer.WaitForNextTickAsync(cancellationTokenSource.Token);
        } while (!found && !cancellationTokenSource.IsCancellationRequested);

        // Assert
        found.Should().BeTrue();

        await _classFixture.GetRequiredService<EventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UpdateUserEventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
        
        await _classFixture.GetRequiredService<UpdateUserCommandBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
    }

    [Fact]
    public async Task SendUpdateUserCommandWithSameLicenseNumberVerifyEntityAndEventFail()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        var entityRepository = _classFixture
            .GetRequiredService<IRepository<User>>();

        var entity = _fixture
            .Build<User>()
            .Create(); 

        await entityRepository.CreateAsync(entity, cancellationTokenSource.Token);

        var licenseNumber = entity.LicenseNumber;

        entity = _fixture
            .Build<User>()
            .Create(); 

        await entityRepository.CreateAsync(entity, cancellationTokenSource.Token);

        var command = _fixture
            .Build<UpdateUserCommand>()
                .With(x => x.Id, entity.Id)
                .With(x => x.LicenseNumber, licenseNumber)
            .Create();

        await _classFixture.GetRequiredService<IPublisher>()
            .PublishCommandAsync(command, cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UpdateUserCommandBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UpdateUserEventBackgroundService>()
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
                    x.Name == typeof(UpdateUserEvent).Name);

            found = commandResult.IsSuccess &&
                eventResult.IsSuccess;

            await periodicTimer.WaitForNextTickAsync(cancellationTokenSource.Token);
        } while (!found && !cancellationTokenSource.IsCancellationRequested);

        // Assert
        found.Should().BeTrue();

        await _classFixture.GetRequiredService<EventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UpdateUserEventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
        
        await _classFixture.GetRequiredService<UpdateUserCommandBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
    }
}
