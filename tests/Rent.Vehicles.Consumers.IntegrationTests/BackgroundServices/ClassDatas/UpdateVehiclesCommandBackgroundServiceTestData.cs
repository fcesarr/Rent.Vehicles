using System.Collections;
using System.Net;

using AutoFixture;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Types;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Projections.Events;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices.ClassDatas;

public class UpdateVehiclesCommandBackgroundServiceTestData : IEnumerable<object[]>
{
    private readonly Fixture _fixture;

    public UpdateVehiclesCommandBackgroundServiceTestData()
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

            var command = _fixture.Build<UpdateVehiclesCommand>()
                .With(x => x.Id, entity.Id)
                .Create();

            return new object[]
            {
                new[]
                {
                    Tuple.Create(nameof(UpdateVehiclesEvent), StatusType.Success),
                    Tuple.Create(nameof(UpdateVehiclesProjectionEvent), StatusType.Success)
                },
                HttpStatusCode.OK, new[] { entity }, command, "/api/vehicle/",
                $"/api/vehicle/{command.LicensePlate}"
            };
        })();
        yield return new Func<object[]>(() =>
        {
            var entity = _fixture.Build<Vehicle>()
                .With(x => x.IsRented, true)
                .Create();

            var command = _fixture.Build<UpdateVehiclesCommand>()
                .With(x => x.Id, entity.Id)
                .Create();

            return new object[]
            {
                new[] { Tuple.Create(nameof(UpdateVehiclesEvent), StatusType.Fail) }, HttpStatusCode.OK,
                new[] { entity }, command, "/api/vehicle/", $"/api/vehicle/{entity.LicensePlate}"
            };
        })();
        yield return new Func<object[]>(() =>
        {
            var entity = _fixture.Build<Vehicle>()
                .With(x => x.IsRented, false)
                .Create();

            var entityHasCreated = _fixture.Build<Vehicle>()
                .Create();

            var command = _fixture.Build<UpdateVehiclesCommand>()
                .With(x => x.Id, entity.Id)
                .With(x => x.LicensePlate, entityHasCreated.LicensePlate)
                .Create();

            return new object[]
            {
                new[] { Tuple.Create(nameof(UpdateVehiclesEvent), StatusType.Fail) }, HttpStatusCode.OK,
                new[] { entity, entityHasCreated }, command, "/api/vehicle/",
                $"/api/vehicle/{entityHasCreated.LicensePlate}"
            };
        })();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
