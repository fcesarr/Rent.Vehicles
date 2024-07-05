
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
using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Extensions;

using Xunit.Abstractions;
using RabbitMQ.Client;
using System.Net;
using System.Text;
using Microsoft.OpenApi.Writers;
using Rent.Vehicles.Services.Responses;
using Amqp.Types;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Mime;
using Rent.Vehicles.Entities.Types;
using System.Collections;
using Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices.ClassDatas;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices;

[Collection(nameof(IntegrationTestWebAppFactoryFixture))]
public class CreateVehiclesCommandBackgroundServiceTests
{
    private readonly Fixture _fixture;

    private readonly IntegrationTestWebAppFactory _integrationTestWebAppFactory;

    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        PropertyNameCaseInsensitive = true
    };

    public CreateVehiclesCommandBackgroundServiceTests(IntegrationTestWebAppFactory integrationTestWebAppFactory)
    {
        _fixture = new Fixture();

        _integrationTestWebAppFactory = integrationTestWebAppFactory;

        _httpClient = _integrationTestWebAppFactory.CreateClient();
    }

    [Theory(DisplayName = $"{nameof(CreateVehiclesCommandBackgroundServiceTests)}.{nameof(SendCreateVehiclesCommandVerifyEventStatusAndStatusCode)}")]
    [ClassData(typeof(CreateVehiclesCommandBackgroundServiceTestData))]
    public async Task SendCreateVehiclesCommandVerifyEventStatusAndStatusCode(int year,
        Tuple<string, StatusType>[] tuples,
        HttpStatusCode statusCode,
        bool createEntity)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        var commandBuilder = _fixture
            .Build<CreateVehiclesCommand>()
            .Without(x => x.Id)
            .With(x => x.Year, () => {
                var random = new Random();
                return random.Next(year, year);
            });
        
        if(createEntity)
        {
            var entity = _fixture
                .Build<Vehicle>()
                .Create(); 

            entity = await _integrationTestWebAppFactory.SaveAsync(entity, cancellationTokenSource.Token);

            commandBuilder = commandBuilder
                .With(x => x.LicensePlate, entity.LicensePlate);
        }

        var command = commandBuilder.Create();

        var json = JsonSerializer.Serialize(command);

		var httpContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

        var response = await _httpClient.PostAsync("/api/vehicle/", httpContent, cancellationToken: cancellationTokenSource.Token);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationTokenSource.Token);

        var commandResponse = JsonSerializer.Deserialize<Rent.Vehicles.Api.Responses.CommandResponse>(responseBody, _options);

        var location = response?.Headers?.Location?.ToString();

        var found = false;

        do
        {
            var locationResponse = await _httpClient.GetAsync(location, cancellationToken: cancellationTokenSource.Token);

            IList<EventResponse> events = [];

            if(locationResponse.StatusCode == HttpStatusCode.OK)
            {
                var locationResponseBody = await locationResponse.Content.ReadAsStringAsync(cancellationTokenSource.Token);
                events = JsonSerializer.Deserialize<IList<EventResponse>>(locationResponseBody, _options) ?? [];
            }

            var entityResponse = await _httpClient.GetAsync($"/api/vehicle/{commandResponse?.Id.ToString()}", cancellationToken: cancellationTokenSource.Token);

            found = events.GroupBy(v => v.SagaId)
                .Where(g => g.Count() == tuples.Length)
                .SelectMany(x => x.ToArray())
                    .AllOrFalseIfEmpty(x => tuples.Any(y => y.Item1 == x.Name && y.Item2 == x.StatusType )) &&
                entityResponse.StatusCode == statusCode;

            await periodicTimer.WaitForNextTickAsync(cancellationTokenSource.Token);
        } while (!found && !cancellationTokenSource.IsCancellationRequested);

        // Assert
        found.Should().BeTrue();

        response?.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}

