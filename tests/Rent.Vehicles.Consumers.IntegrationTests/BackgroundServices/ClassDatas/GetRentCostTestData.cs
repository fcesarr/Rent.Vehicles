using System.Collections;
using System.Net;

using AutoFixture;

using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices.ClassDatas;

public class GetRentCostTestData : IEnumerable<object[]>
{
    private readonly Fixture _fixture;

    public GetRentCostTestData()
    {
        _fixture = new Fixture();
    }

    public IEnumerator<object[]> GetEnumerator()
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
            .With(x => x.EstimatedDate, startDate.AddDays(numberOfDays))
            .With(x => x.UserId, user.Id)
            .With(x => x.User, user)
            .With(x => x.VehicleId, vehicle.Id)
            .With(x => x.Vehicle, vehicle)
            .With(x => x.IsActive, true)
            .Without(x => x.Updated)
            .Create();

        var rentPlane = _fixture.Build<RentalPlane>()
            .With(x => x.NumberOfDays, entity.NumberOfDays)
            .With(x => x.DailyCost, entity.DailyCost)
            .With(x => x.PreEndDatePercentageFine, entity.PreEndDatePercentageFine)
            .With(x => x.PostEndDateFine, entity.PostEndDateFine)
            .Create();

        yield return new Func<object[]>(() =>
        {
            return new object[]
            {
                HttpStatusCode.OK, new Entity[] { entity, rentPlane },
                $"/api/rent/cost/{entity.Id}/{entity.EstimatedDate.ToString("yyyy-MM-ddTHH:mm:ss zzz")}"
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
                .With(x => x.EstimatedDate, startDate.AddDays(numberOfDays))
                .With(x => x.UserId, user.Id)
                .With(x => x.User, user)
                .With(x => x.VehicleId, vehicle.Id)
                .With(x => x.Vehicle, vehicle)
                .With(x => x.IsActive, true)
                .Without(x => x.Updated)
                .Create();

            var rentPlane = _fixture.Build<RentalPlane>()
                .With(x => x.NumberOfDays, entity.NumberOfDays)
                .With(x => x.DailyCost, entity.DailyCost)
                .With(x => x.PreEndDatePercentageFine, entity.PreEndDatePercentageFine)
                .With(x => x.PostEndDateFine, entity.PostEndDateFine)
                .Create();

            return new object[]
            {
                HttpStatusCode.OK, new Entity[] { entity, rentPlane },
                $"/api/rent/cost/{entity.Id}/{entity.EstimatedDate.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss zzz")}"
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
                .With(x => x.EstimatedDate, startDate.AddDays(numberOfDays))
                .With(x => x.UserId, user.Id)
                .With(x => x.User, user)
                .With(x => x.VehicleId, vehicle.Id)
                .With(x => x.Vehicle, vehicle)
                .With(x => x.IsActive, true)
                .Without(x => x.Updated)
                .Create();

            var rentPlane = _fixture.Build<RentalPlane>()
                .With(x => x.NumberOfDays, entity.NumberOfDays)
                .With(x => x.DailyCost, entity.DailyCost)
                .With(x => x.PreEndDatePercentageFine, entity.PreEndDatePercentageFine)
                .With(x => x.PostEndDateFine, entity.PostEndDateFine)
                .Create();

            return new object[]
            {
                HttpStatusCode.BadRequest, new Entity[] { entity, rentPlane },
                $"/api/rent/cost/{entity.Id}/{DateTime.Now.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ss zzz")}"
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
                .With(x => x.EstimatedDate, startDate.AddDays(numberOfDays))
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

            return new object[]
            {
                HttpStatusCode.BadRequest, new Entity[] { entity, rentPlane },
                $"/api/rent/cost/{entity.Id}/{entity.EstimatedDate.ToString("yyyy-MM-ddTHH:mm:ss zzz")}"
            };
        })();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
