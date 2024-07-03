
using System.Reflection;

using AutoFixture;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Commands.BackgroundServices;
using Rent.Vehicles.Consumers.Events.BackgroundServices;
using Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices;

[Collection(nameof(CommonCollectionFixture))]
public class UpdateUserLicenseImageCommandBackgroundServiceTests : CommandBackgroundServiceTests
{
    private readonly Fixture _fixture;
    
    public UpdateUserLicenseImageCommandBackgroundServiceTests(CommonFixture classFixture) : base(classFixture)
    {
        _fixture = new Fixture();
        
        _queues.Add("UpdateUserLicenseImageCommand");
        _queues.Add("UpdateUserLicenseImageEvent");
        _queues.Add("UploadUserLicenseImageEvent");
    }


    public static async Task<string> GetBase64StringAsync(string name, CancellationToken cancellationToken = default)
    {
        name = $"Rent.Vehicles.Consumers.IntegrationTests.Images.{name}";

        var result = string.Empty;

        var assembly = Assembly.GetExecutingAssembly();

        if(assembly.GetManifestResourceStream(name) is Stream stream)
        {
            using StreamReader reader = new StreamReader(stream);
            result = await reader.ReadToEndAsync(cancellationToken);
            await stream.DisposeAsync();
        }

        return result;
    }

    [Theory]
    [InlineData("pngBase64String", "299d8904e674332b32d2fc1c7fcf26ce.png")]
    [InlineData("bmpBase64String", "c23ff0fb8955b91ef38bf12ffcd80c00.bmp")]
    public async Task UpdateUserLicenseImageCommandVerifyImageAreUplodad(string name, string expected)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(90));

        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        var userRepository = _classFixture
            .GetRequiredService<IRepository<User>>();

        var entity = _fixture
            .Build<User>()
            .Create(); 

        await userRepository.CreateAsync(entity, cancellationTokenSource.Token);

        var base64String = await GetBase64StringAsync(name, cancellationTokenSource.Token);

        var command = _fixture
            .Build<UpdateUserLicenseImageCommand>()
                .With(x => x.Id, entity.Id)
                .With(x => x.LicenseImage, base64String)
            .Create();

        var streamUploadService = _classFixture.GetRequiredService<IStreamUploadService>();

        streamUploadService.Bytes = Array.Empty<Byte>();

        await _classFixture.GetRequiredService<IPublisher>()
            .PublishCommandAsync(command, cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UpdateUserLicenseImageCommandBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UpdateUserLicenseImageEventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UploadUserLicenseImageEventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        var commandDataService = _classFixture.GetRequiredService<ICommandDataService>();

        var found = false;

        do
        {
            var commandResult = await commandDataService
                .GetAsync(x => x.SagaId == command.SagaId, cancellationTokenSource.Token);

            var nameResult = await streamUploadService
                .GetPathAsync(base64String, cancellationTokenSource.Token);

            found = commandResult.IsSuccess &&
                nameResult.IsSuccess &&
                nameResult.Value! == expected &&
                streamUploadService.Bytes.SequenceEqual(Convert.FromBase64String(base64String));

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

    [Fact]
    public async Task UpdateUserLicenseImageCommandVerifyImageNotUploaded()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(90));

        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        var userRepository = _classFixture
            .GetRequiredService<IRepository<User>>();

        var entity = _fixture
            .Build<User>()
            .Create(); 

        await userRepository.CreateAsync(entity, cancellationTokenSource.Token);

        var base64String = await GetBase64StringAsync("jpgBase64String", cancellationTokenSource.Token);

        var command = _fixture
            .Build<UpdateUserLicenseImageCommand>()
                .With(x => x.Id, entity.Id)
                .With(x => x.LicenseImage, base64String)
            .Create();
        
        var streamUploadService = _classFixture.GetRequiredService<IStreamUploadService>();

        streamUploadService.Bytes = Array.Empty<Byte>();

        await _classFixture.GetRequiredService<IPublisher>()
            .PublishCommandAsync(command, cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UpdateUserLicenseImageCommandBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UpdateUserLicenseImageEventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        await _classFixture.GetRequiredService<UploadUserLicenseImageEventBackgroundService>()
            .StartAsync(cancellationTokenSource.Token);

        var commandDataService = _classFixture.GetRequiredService<ICommandDataService>();

        var found = false;

        do
        {
            var commandResult = await commandDataService
                .GetAsync(x => x.SagaId == command.SagaId, cancellationTokenSource.Token);

            var nameResult = await streamUploadService
                .GetPathAsync(base64String, cancellationTokenSource.Token);

            found = commandResult.IsSuccess &&
                !nameResult.IsSuccess &&
                nameResult.Exception is not null &&
                streamUploadService.Bytes.Length == 0;

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
}
