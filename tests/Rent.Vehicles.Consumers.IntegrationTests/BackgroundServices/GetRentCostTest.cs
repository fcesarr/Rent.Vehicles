using System.Net;
using System.Text.Json;

using AutoFixture;

using FluentAssertions;

using Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices.ClassDatas;
using Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;
using Rent.Vehicles.Services.Extensions;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices;


[Collection(nameof(IntegrationTestWebAppFactoryFixture))]
public class GetRentCostTest : IAsyncLifetime
{
    private readonly Fixture _fixture;

    private readonly IntegrationTestWebAppFactory _integrationTestWebAppFactory;

    private readonly HttpClient _httpClient;

    public GetRentCostTest(IntegrationTestWebAppFactory integrationTestWebAppFactory)
    {
        _fixture = new Fixture();

        _integrationTestWebAppFactory = integrationTestWebAppFactory;

        _httpClient = _integrationTestWebAppFactory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await _integrationTestWebAppFactory.ResetDatabaseAsync();
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    [Theory(DisplayName = $"{nameof(GetRentCostTest)}.{nameof(GetCostAsync)}")]
    [ClassData(typeof(GetRentCostTestData))]
    public async Task GetCostAsync(HttpStatusCode statusCode,
        IEnumerable<dynamic> entities,
        string endpointGet)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(180));

        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        foreach (var entity in entities)
        {
            _ = await _integrationTestWebAppFactory.SaveAsync(entity, cancellationTokenSource.Token);

            if(entity.GetType().Name != "RentalPlane")
                _ = await _integrationTestWebAppFactory.SaveAsync(ToExtension.ToProjection(entity), cancellationTokenSource.Token);
        }

        var response = await _httpClient.GetAsync(endpointGet, cancellationToken: cancellationTokenSource.Token);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationTokenSource.Token);

        response?.StatusCode.Should().Be(statusCode);
    }
}
