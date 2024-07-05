using AutoFixture;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MongoDB.Driver;

using RabbitMQ.Client;

using Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Interfaces;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices;

public abstract class CommandBackgroundServiceTests : IAsyncLifetime
{
    protected readonly CommonFixture _classFixture;

    protected readonly IList<string> _queues = new List<string>();

    public CommandBackgroundServiceTests(CommonFixture classFixture)
    {
        _classFixture = classFixture;
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        var connection = _classFixture.GetRequiredService<IConnection>();
        
        var model = connection.CreateModel();

        foreach (var queue in _queues)
        {
            model.QueuePurge(queue);
        }

        model.Close();

        await _classFixture.ResetDatabaseAsync();
    }
}
