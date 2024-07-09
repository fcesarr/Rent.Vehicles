using System.Collections;
using System.Net;

using AutoFixture;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Types;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Projections.Events;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices.ClassDatas;

public class CreateVehiclesCommandBackgroundServiceTestData : IEnumerable<object[]>
{
    private readonly Fixture _fixture;

    public CreateVehiclesCommandBackgroundServiceTestData()
    {
        _fixture = new Fixture();
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new Func<object[]>(() => {
            var command = _fixture.Build<CreateVehiclesCommand>()
                    .With(x => x.Year, 2024)
                .Create();

            return new object[]{
                new Tuple<string, StatusType>[] { 
                    Tuple.Create(nameof(CreateVehiclesEvent), StatusType.Success),
                    Tuple.Create(nameof(CreateVehiclesForSpecificYearEvent), StatusType.Success),
                    Tuple.Create(nameof(CreateVehiclesProjectionEvent), StatusType.Success),
                    Tuple.Create(nameof(CreateVehiclesForSpecificYearProjectionEvent), StatusType.Success)
                },
                HttpStatusCode.OK,
                Array.Empty<Vehicle>(),
                command,
                "/api/vehicle/",
                $"/api/vehicle/{command.LicensePlate}"
            };
        })();
        yield return new Func<object[]>(() => {
            var command = _fixture.Build<CreateVehiclesCommand>()
                    .With(x => x.Year, 2025)
                .Create();

            return new object[]{
                new Tuple<string, StatusType>[] { 
                    Tuple.Create(nameof(CreateVehiclesEvent), StatusType.Success),
                    Tuple.Create(nameof(CreateVehiclesForSpecificYearEvent), StatusType.Fail),
                    Tuple.Create(nameof(CreateVehiclesProjectionEvent), StatusType.Success)
                },
                HttpStatusCode.OK,
                Array.Empty<Vehicle>(),
                command,
                "/api/vehicle/",
                $"/api/vehicle/{command.LicensePlate}"
            };
        })();
        yield return new Func<object[]>(() => {
            var entity = _fixture
                .Create<Vehicle>();

            var command = _fixture.Build<CreateVehiclesCommand>()
                    .With(x => x.LicensePlate, entity.LicensePlate)
                .Create();

            return new object[]{
                new Tuple<string, StatusType>[] { 
                    Tuple.Create(nameof(CreateVehiclesEvent), StatusType.Fail)
                },
                HttpStatusCode.NotFound,
                new Vehicle[]{entity},
                command,
                "/api/vehicle/",
                $"/api/vehicle/{command.LicensePlate}"
            };
        })();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
