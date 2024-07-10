using System.Collections;
using System.Net;

using AutoFixture;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Types;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Projections.Events;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices.ClassDatas;

public class UpdateRentCommandBackgroundServiceTestData : IEnumerable<object[]>
{
    private readonly Fixture _fixture;

    public UpdateRentCommandBackgroundServiceTestData()
    {
        _fixture = new Fixture();
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new Func<object[]>(() =>
        {
            var user = _fixture.Build<User>()
                .Create();

            var vehicle = _fixture.Build<Vehicle>()
                .With(x => x.IsRented, true)
                .Create();

            var startDate = DateTime.Now.Date.AddDays(1);

            var numberOfDays = _fixture.Create<int>();

            var entity = _fixture.Build<Entities.Rent>()
                .With(x => x.StartDate, startDate)
                .With(x => x.NumberOfDays, numberOfDays)
                .With(x => x.EndDate, startDate.AddDays(numberOfDays))
                .With(x => x.UserId, user.Id)
                .With(x => x.User, user)
                .With(x => x.VehicleId, vehicle.Id)
                .With(x => x.Vehicle, vehicle)
                .Create();

            var rentPlane = _fixture.Build<RentalPlane>()
                .With(x => x.NumberOfDays, entity.NumberOfDays)
                .With(x => x.DailyCost, entity.DailyCost)
                .With(x => x.PreEndDatePercentageFine, entity.PreEndDatePercentageFine)
                .With(x => x.PostEndDateFine, entity.PostEndDateFine)
                .Create();

            var command = _fixture.Build<UpdateRentCommand>()
                .With(x => x.Id, entity.Id)
                .With(x => x.EstimatedDate, startDate.AddDays(numberOfDays))
                .Create();

            return new object[]
            {
                new[]
                {
                    Tuple.Create(nameof(UpdateRentEvent), StatusType.Success),
                    Tuple.Create(nameof(UpdateRentProjectionEvent), StatusType.Success)
                },
                HttpStatusCode.OK, new Entity[] { entity, rentPlane }, command, "/api/rent/",
                $"/api/rent/{command.Id.ToString()}"
            };
        })();
        yield return new Func<object[]>(() =>
        {
            var user = _fixture.Build<User>()
                .Create();

            var vehicle = _fixture.Build<Vehicle>()
                .With(x => x.IsRented, true)
                .Create();

            var startDate = DateTime.Now.Date.AddDays(1);

            var numberOfDays = _fixture.Create<int>();

            var entity = _fixture.Build<Entities.Rent>()
                .With(x => x.StartDate, startDate)
                .With(x => x.NumberOfDays, numberOfDays)
                .With(x => x.EndDate, startDate.AddDays(numberOfDays))
                .With(x => x.UserId, user.Id)
                .With(x => x.User, user)
                .With(x => x.VehicleId, vehicle.Id)
                .With(x => x.Vehicle, vehicle)
                .Create();

            var rentPlane = _fixture.Build<RentalPlane>()
                .With(x => x.NumberOfDays, entity.NumberOfDays)
                .With(x => x.DailyCost, entity.DailyCost)
                .With(x => x.PreEndDatePercentageFine, entity.PreEndDatePercentageFine)
                .With(x => x.PostEndDateFine, entity.PostEndDateFine)
                .Create();

            var command = _fixture.Build<UpdateRentCommand>()
                .With(x => x.Id, entity.Id)
                .With(x => x.EstimatedDate, startDate.AddDays(-1))
                .Create();

            return new object[]
            {
                new[] { Tuple.Create(nameof(UpdateRentEvent), StatusType.Fail) }, HttpStatusCode.OK,
                new Entity[] { entity, rentPlane }, command, "/api/rent/", $"/api/rent/{command.Id.ToString()}"
            };
        })();
        yield return new Func<object[]>(() =>
        {
            var user = _fixture.Build<User>()
                .Create();

            var vehicle = _fixture.Build<Vehicle>()
                .With(x => x.IsRented, true)
                .Create();

            var startDate = DateTime.Now.Date.AddDays(1);

            var numberOfDays = _fixture.Create<int>();

            var entity = _fixture.Build<Entities.Rent>()
                .With(x => x.StartDate, startDate)
                .With(x => x.NumberOfDays, numberOfDays)
                .With(x => x.EndDate, startDate.AddDays(numberOfDays))
                .With(x => x.UserId, user.Id)
                .With(x => x.User, user)
                .With(x => x.VehicleId, vehicle.Id)
                .With(x => x.Vehicle, vehicle)
                .With(x => x.IsActive, false)
                .Create();

            var rentPlane = _fixture.Build<RentalPlane>()
                .With(x => x.NumberOfDays, entity.NumberOfDays)
                .With(x => x.DailyCost, entity.DailyCost)
                .With(x => x.PreEndDatePercentageFine, entity.PreEndDatePercentageFine)
                .With(x => x.PostEndDateFine, entity.PostEndDateFine)
                .Create();

            var command = _fixture.Build<UpdateRentCommand>()
                .With(x => x.Id, entity.Id)
                .With(x => x.EstimatedDate, startDate.AddDays(numberOfDays))
                .Create();

            return new object[]
            {
                new[] { Tuple.Create(nameof(UpdateRentEvent), StatusType.Fail) }, HttpStatusCode.OK,
                new Entity[] { entity, rentPlane }, command, "/api/rent/", $"/api/rent/{command.Id.ToString()}"
            };
        })();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
