
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

[Collection(nameof(CommonCollection))]
public class UpdateUserLicenseImageCommandBackgroundServiceTests : CommandBackgroundServiceTests<UpdateUserLicenseImageCommandBackgroundService, UpdateUserLicenseImageCommand>
{
    private readonly Fixture _fixture;

    public UpdateUserLicenseImageCommandBackgroundServiceTests(CommonFixture classFixture, ITestOutputHelper output) : base(classFixture, output)
    {
        _fixture = new Fixture();
    }

    protected override UpdateUserLicenseImageCommand GetCommand()
    {
        return _fixture
            .Build<UpdateUserLicenseImageCommand>()
            .Create();
    }
}
