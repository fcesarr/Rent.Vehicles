using System.Collections;
using System.Net;

using AutoFixture;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Types;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Projections.Events;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices.ClassDatas;

public class DeleteVehiclesCommandBackgroundServiceTestData : IEnumerable<object[]>
{
    private readonly Fixture _fixture;

    public DeleteVehiclesCommandBackgroundServiceTestData()
    {
        _fixture = new Fixture();
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new Func<object[]>(() => {
            var entity = _fixture.Build<Vehicle>()
                    .With(x => x.IsRented, false)
                .Create();

            return new object[] { 
                new Tuple<string, StatusType>[] { 
                    Tuple.Create(nameof(DeleteVehiclesEvent), StatusType.Success),
                    Tuple.Create(nameof(DeleteVehiclesProjectionEvent), StatusType.Success),
                },
                HttpStatusCode.NotFound,
                entity,
                $"/api/vehicle/{entity.Id.ToString()}",
                $"/api/vehicle/{entity.LicensePlate}"
            };
        })();
        yield return new Func<object[]>(() => {
            var entity = _fixture.Build<Vehicle>()
                    .With(x => x.IsRented, true)
                .Create();

            return new object[] { 
                new Tuple<string, StatusType>[] { 
                    Tuple.Create(nameof(DeleteVehiclesEvent), StatusType.Fail)
                },
                HttpStatusCode.OK,
                entity,
                $"/api/vehicle/{entity.Id.ToString()}",
                $"/api/vehicle/{entity.LicensePlate}"
            };
        })();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
