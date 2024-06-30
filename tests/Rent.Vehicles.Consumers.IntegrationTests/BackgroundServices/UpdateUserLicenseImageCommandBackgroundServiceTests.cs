
using AutoFixture;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Rent.Vehicles.Consumers.Commands.BackgroundServices;
using Rent.Vehicles.Consumers.Events.BackgroundServices;
using Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Interfaces;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices;

[Collection(nameof(CommonCollection))]
public class UpdateUserLicenseImageCommandBackgroundServiceTests
{
    private readonly Fixture _fixture;

    private readonly CommonFixture _classFixture;

    public UpdateUserLicenseImageCommandBackgroundServiceTests(CommonFixture classFixture)
    {
        _fixture = new Fixture();
        _classFixture = classFixture;
    }

    [Fact]
    public async Task SendCommandAndVerifyCommandIsCreatedInDatabase()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        var command = _fixture
            .Build<UpdateUserLicenseImageCommand>()
                .With(x => x.LicenseImage, _base64String)
            .Create();

        await _classFixture.GetRequiredService<IPublisher>()
            .PublishCommandAsync(command, cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UpdateUserLicenseImageCommandBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UpdateUserLicenseImageEventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UploadUserLicenseImageEventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        var commandDataService = _classFixture.GetRequiredService<ICommandDataService>();

        var streamUploadService = _classFixture.GetRequiredService<IStreamUploadService>();

        var found = false;

        do
        {
            var commandResult = await commandDataService
                .GetAsync(x => x.SagaId == command.SagaId, cancellationTokenSource.Token);

            var nameResult = await streamUploadService
                .GetNameAsync(_base64String, cancellationTokenSource.Token);

            found = commandResult.IsSuccess &&
                nameResult.IsSuccess &&
                nameResult.Value! == "299d8904e674332b32d2fc1c7fcf26ce.png" &&
                streamUploadService.Bytes.SequenceEqual(Convert.FromBase64String(_base64String));

            await periodicTimer.WaitForNextTickAsync(cancellationTokenSource.Token);
        } while (!found && !cancellationTokenSource.IsCancellationRequested);

        // Assert
        found.Should().BeTrue();

        await _classFixture.GetRequiredService<UploadUserLicenseImageEventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UpdateUserLicenseImageEventBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
        
        await _classFixture.GetRequiredService<UpdateUserLicenseImageCommandBackgroundService>()
            .StopAsync(cancellationTokenSource.Token);
    }

    private string _base64String { get; } = "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";
}
