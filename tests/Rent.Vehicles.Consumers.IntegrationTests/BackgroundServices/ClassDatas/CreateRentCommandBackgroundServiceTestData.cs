using System.Collections;
using System.Net;

using AutoFixture;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Types;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Projections.Events;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices.ClassDatas;

public class CreateRentCommandBackgroundServiceTestData : IEnumerable<object[]>
{
    private readonly Fixture _fixture;

    public CreateRentCommandBackgroundServiceTestData()
    {
        _fixture = new Fixture();
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new Func<object[]>(() => {
            var user = _fixture.Create<User>();

            var rentPlane = _fixture.Create<RentalPlane>();

            var vehicle = _fixture.Build<Vehicle>()
                    .With(x => x.IsRented, false)
                .Create();

            var command = _fixture.Build<CreateRentCommand>()
                    .With(x => x.UserId, user.Id)
                    .With(x => x.RentPlaneId, rentPlane.Id)
                .Create();

            return new object[]{
                new Tuple<string, StatusType>[] { 
                    Tuple.Create(nameof(CreateRentEvent), StatusType.Success),
                    Tuple.Create(nameof(CreateRentProjectionEvent), StatusType.Success),
                },
                HttpStatusCode.OK,
                new Entity[]{ user, rentPlane, vehicle },
                command,
                "/api/rent/"
            };
        })();
        yield return new Func<object[]>(() => {
            var user = _fixture.Create<User>();

            var rentPlane = _fixture.Create<RentalPlane>();

            var vehicle = _fixture.Build<Vehicle>()
                    .With(x => x.IsRented, true)
                .Create();

            var command = _fixture.Build<CreateRentCommand>()
                    .With(x => x.UserId, user.Id)
                    .With(x => x.RentPlaneId, rentPlane.Id)
                .Create();

            return new object[]{
                new Tuple<string, StatusType>[] { 
                    Tuple.Create(nameof(CreateRentEvent), StatusType.Fail)
                },
                HttpStatusCode.NotFound,
                new Entity[]{ user, rentPlane, vehicle },
                command,
                "/api/rent/"
            };
        })();
        yield return new Func<object[]>(() => {
            var rentPlane = _fixture.Create<RentalPlane>();

            var vehicle = _fixture.Build<Vehicle>()
                    .With(x => x.IsRented, false)
                .Create();

            var command = _fixture.Build<CreateRentCommand>()
                    .With(x => x.RentPlaneId, rentPlane.Id)
                .Create();

            return new object[]{
                new Tuple<string, StatusType>[] { 
                    Tuple.Create(nameof(CreateRentEvent), StatusType.Fail)
                },
                HttpStatusCode.NotFound,
                new Entity[]{ rentPlane, vehicle },
                command,
                "/api/rent/"
            };
        })();
        yield return new Func<object[]>(() => {
            var user = _fixture.Create<User>();

            var rentPlane = _fixture.Create<RentalPlane>();

            var command = _fixture.Build<CreateRentCommand>()
                    .With(x => x.UserId, user.Id)
                    .With(x => x.RentPlaneId, rentPlane.Id)
                .Create();

            return new object[]{
                new Tuple<string, StatusType>[] { 
                    Tuple.Create(nameof(CreateRentEvent), StatusType.Fail)
                },
                HttpStatusCode.NotFound,
                new Entity[]{ user, rentPlane },
                command,
                "/api/rent/"
            };
        })();
        yield return new Func<object[]>(() => {
            var user = _fixture.Create<User>();

            var rentPlane = _fixture.Create<RentalPlane>();

            var vehicle = _fixture.Build<Vehicle>()
                    .With(x => x.IsRented, false)
                .Create();

            var command = _fixture.Build<CreateRentCommand>()
                    .With(x => x.UserId, user.Id)
                .Create();

            return new object[]{
                new Tuple<string, StatusType>[] { 
                    Tuple.Create(nameof(CreateRentEvent), StatusType.Fail)
                },
                HttpStatusCode.NotFound,
                new Entity[]{ user, vehicle },
                command,
                "/api/rent/"
            };
        })();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
