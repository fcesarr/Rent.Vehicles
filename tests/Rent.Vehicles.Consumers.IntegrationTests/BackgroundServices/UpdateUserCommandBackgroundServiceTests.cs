
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

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices;

[Collection(nameof(CommonCollectionFixture))]
public class UpdateUserCommandBackgroundServiceTests : CommandBackgroundServiceTests
{
    private readonly Fixture _fixture;

    public UpdateUserCommandBackgroundServiceTests(CommonFixture classFixture) : base(classFixture)
    {
        _fixture = new Fixture();
    }
}
