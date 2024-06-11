using System.Diagnostics.CodeAnalysis;

using AutoFixture;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using RabbitMQ.Client;

using Rent.Vehicles.Consumers.IntegrationTests.Configuration;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;

[ExcludeFromCodeCoverage]
public class ConsumerFixture<TBackgroundService, TCommand, TEntity> : IDisposable 
    where TBackgroundService : BackgroundService 
    where TCommand : Messages.Command
    where TEntity : Entities.Command
{
    private TBackgroundService? _backgroundService;

    private bool _started;

    private ServiceProvider? _serviceProfile;

    private readonly Fixture? _fixture;

    private IModel? _model;

    private ISerializer? _serializer;

    private IService<TEntity>? _service;

    public void Init(ITestOutputHelper output)
    {
        _serviceProfile = ServiceProviderManager
            .GetInstance(output)
            .GetServiceProvider();
    
        _backgroundService = _serviceProfile.GetRequiredService<TBackgroundService>();
        _model = _serviceProfile.GetRequiredService<IModel>();
        _serializer = _serviceProfile.GetRequiredService<ISerializer>();
        _service = _serviceProfile.GetRequiredService<IService<TEntity>>();
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

    public async Task SendCommandAsync(TCommand message, CancellationToken cancellationToken)
    {
        var bytes = await _serializer!.SerializeAsync(message, cancellationToken);

        _model?.BasicPublish(exchange: string.Empty,
            routingKey: typeof(TCommand).Name,
            basicProperties: null,
            body: bytes);
    }

    public uint QueueCount(string queueName)
    {
        return _model?.QueueDeclarePassive(queueName).MessageCount ?? default;
    }

    public async Task<TEntity?> GetCommandAsync(string sql, IDictionary<string, dynamic> parameters)
    {
        return await _service!.GetAsync(sql, parameters);
    }

    public void Dispose()
    {
        _backgroundService?.Dispose();
        _serviceProfile?.Dispose();
        GC.SuppressFinalize(this);
    }
}