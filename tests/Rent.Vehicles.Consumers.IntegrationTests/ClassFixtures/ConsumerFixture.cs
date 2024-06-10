using System.Diagnostics.CodeAnalysis;

using AutoFixture;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using RabbitMQ.Client;

using Rent.Vehicles.Consumers.IntegrationTests.Configuration;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;

[ExcludeFromCodeCoverage]
public class ConsumerFixture<TBackgroundService, TMessage> : IDisposable where TBackgroundService : BackgroundService where TMessage : Message
{
    private TBackgroundService? _backgroundService;

    private bool _started;

    private ServiceProvider? _serviceProfile;

    private readonly Fixture? _fixture;

    private IModel? _model;

    private ISerializer? _serializer;

    public void Init(ITestOutputHelper output)
    {
        _serviceProfile = ServiceProviderManager
            .GetInstance(output)
            .GetServiceProvider();
    
        _backgroundService = _serviceProfile.GetRequiredService<TBackgroundService>();
        _model = _serviceProfile.GetRequiredService<IModel>();
        _serializer = _serviceProfile.GetRequiredService<ISerializer>();
    }

    public Fixture GetFixture() => _fixture ?? new Fixture();

    public async Task StartWorkerAsync(CancellationToken cancellationToken)
    {
        if (_started) return;

        if (_backgroundService != null)
            await _backgroundService.StartAsync(cancellationToken);

        _started = true;
    }

    public async Task StopWorkerAsync(CancellationToken cancellationToken)
    {
        if (!_started) return;

        if (_backgroundService != null)
            await _backgroundService.StopAsync(cancellationToken);

        _started = false;
    }

    public async Task SendCommandAsync(TMessage message, CancellationToken cancellationToken)
    {
        var bytes = await _serializer!.SerializeAsync(message, cancellationToken);

        _model?.BasicPublish(exchange: string.Empty,
            routingKey: typeof(TMessage).Name,
            basicProperties: null,
            body: bytes);
    }

    public uint QueueCount(string queueName)
    {
        return _model?.QueueDeclarePassive(queueName).MessageCount ?? default;
    }

    public void Dispose()
    {
        _backgroundService?.Dispose();
        _serviceProfile?.Dispose();
        GC.SuppressFinalize(this);
    }
}