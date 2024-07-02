
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

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices;

[Collection(nameof(CommonCollectionFixture))]
public class CreateUserCommandBackgroundServiceTests : CommandBackgroundServiceTests
{
    private readonly Fixture _fixture;

    public CreateUserCommandBackgroundServiceTests(CommonFixture classFixture) : base(classFixture)
    {
        _fixture = new Fixture();

        _queues.Add("CreateUserCommand");
        _queues.Add("CreateUserEvent");
        _queues.Add("CreateUserProjectionEvent");
        _queues.Add("Event");
    }

    private static Expression<Func<TEntity, bool>> GetPredicate<TEntity>(Guid id) where TEntity : Entity => x => x.Id == id;

    private static Expression<Func<User, bool>> GetPredicate(CreateUserCommand command)
    {
        var predicate = GetPredicate<User>(command.Id);

        return predicate.And(x => x.Name == command.Name &&
            x.Number == command.Number &&
            x.Birthday == command.Birthday &&
            x.LicenseNumber == command.LicenseNumber &&
            x.LicenseType == (Entities.Types.LicenseType)command.LicenseType);
    }

    private static Expression<Func<UserProjection, bool>> GetProjectionPredicate(CreateUserCommand command)
    {
        var predicate = GetPredicate<UserProjection>(command.Id);

        return predicate.And(x => x.Name == command.Name &&
            x.Number == command.Number &&
            x.Birthday == command.Birthday &&
            x.LicenseNumber == command.LicenseNumber &&
            x.LicenseType == (Entities.Types.LicenseType)command.LicenseType);
    }

    [Fact]
    public async Task SendCreateUserCommandWithImageFormatNotSupportedVerifyEntityAndEventFail()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(90));

        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        var base64String = await UpdateUserLicenseImageCommandBackgroundServiceTests
            .GetBase64StringAsync("jpgBase64String", cancellationTokenSource.Token);

        var command = _fixture
            .Build<CreateUserCommand>()
                .With(x => x.LicenseImage, base64String)
            .Create();

        await _classFixture.GetRequiredService<IPublisher>()
            .PublishCommandAsync(command, cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateUserCommandBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateUserEventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<EventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        var commandDataService = _classFixture
            .GetRequiredService<ICommandDataService>();

        var entityDataService = _classFixture
            .GetRequiredService<IUserDataService>();

        var projectionDataService = _classFixture
            .GetRequiredService<IUserProjectionDataService>();

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
                    x.Message.Contains("Extensão não suportada.") &&
                    x.Name == typeof(CreateUserEvent).Name);

            found = commandResult.IsSuccess &&
                eventResult.IsSuccess;

            await periodicTimer.WaitForNextTickAsync(cancellationTokenSource.Token);
        } while (!found && !cancellationTokenSource.IsCancellationRequested);

        // Assert
        found.Should().BeTrue();

        await _classFixture.GetRequiredService<EventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateUserEventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
        
        await _classFixture.GetRequiredService<CreateUserCommandBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
    }

    [Theory]
    [InlineData("pngBase64String")]
    [InlineData("bmpBase64String")]
    public async Task SendCreateUserCommandVerifyEntityAndProjectionAreSaved(string name)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(90));

        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        var base64String = await UpdateUserLicenseImageCommandBackgroundServiceTests
            .GetBase64StringAsync(name, cancellationTokenSource.Token);

        var command = _fixture
            .Build<CreateUserCommand>()
                .With(x => x.LicenseImage, base64String)
            .Create();

        await _classFixture.GetRequiredService<IPublisher>()
            .PublishCommandAsync(command, cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateUserCommandBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateUserEventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateUserProjectionEventBackgroundService>()
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
                projectionResult.IsSuccess;

            await periodicTimer.WaitForNextTickAsync(cancellationTokenSource.Token);
        } while (!found && !cancellationTokenSource.IsCancellationRequested);

        // Assert
        found.Should().BeTrue();

        await _classFixture.GetRequiredService<CreateUserProjectionEventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<CreateUserEventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
        
        await _classFixture.GetRequiredService<CreateUserCommandBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
    }
}
