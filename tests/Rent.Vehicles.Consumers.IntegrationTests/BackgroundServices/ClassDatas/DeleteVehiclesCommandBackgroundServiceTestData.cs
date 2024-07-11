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
        yield return new Func<object[]>(() =>
        {
            var entity = _fixture.Build<Vehicle>()
                .With(x => x.IsRented, false)
                .Create();

            return new object[]
            {
                new[]
                {
                    Tuple.Create(nameof(DeleteVehiclesEvent), StatusType.Success),
                    Tuple.Create(nameof(DeleteVehiclesProjectionEvent), StatusType.Success)
                },
                HttpStatusCode.NotFound, new[] { entity }, $"/api/vehicle/{entity.Id.ToString()}",
                $"/api/vehicle/{entity.LicensePlate}"
            };
        })();
        yield return new Func<object[]>(() =>
        {
            var entity = _fixture.Build<Vehicle>()
                .With(x => x.IsRented, true)
                .Create();

            return new object[]
            {
                new[] { Tuple.Create(nameof(DeleteVehiclesEvent), StatusType.Fail) }, HttpStatusCode.OK, new[] { entity },
                $"/api/vehicle/{entity.Id.ToString()}", $"/api/vehicle/{entity.LicensePlate}"
            };
        })();
        yield return new Func<object[]>(() =>
        {
            var entity = _fixture.Build<Vehicle>()
                .With(x => x.IsRented, true)
                .Create();
            
            var rent = _fixture.Build<Entities.Rent>()
                    .With(x => x.VehicleId, entity.Id)
                    .Without(x => x.Vehicle)
                .Create();

            return new object[]
            {
                new[] { Tuple.Create(nameof(DeleteVehiclesEvent), StatusType.Fail) }, HttpStatusCode.OK, new Entity[] { entity, rent },
                $"/api/vehicle/{entity.Id.ToString()}", $"/api/vehicle/{entity.LicensePlate}"
            };
        })();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
