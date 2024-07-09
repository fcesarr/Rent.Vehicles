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
        yield return new Func<object[]>(() => {
            var startDate = DateTime.Now.Date.AddDays(1);

            var numberOfDays = _fixture.Create<int>();

            var entity = _fixture.Build<Entities.Rent>()
                    .With(x => x.StartDate, startDate)
                    .With(x => x.NumberOfDays, numberOfDays)
                    .With(x => x.EndDate, startDate.AddDays(numberOfDays))
                .Create();

            var rentPlane = _fixture.Build<RentalPlane>()
                    .With(x => x.NumberOfDays, entity.NumberOfDays)
                    .With(x => x.DailyCost, entity.DailyCost)
                    .With(x => x.PreEndDatePercentageFine, entity.PreEndDatePercentageFine)
                    .With(x => x.PostEndDateFine, entity.PostEndDateFine)
                .Create();

            var vehicle = _fixture.Build<Vehicle>()
                    .With(x => x.Id, entity.VehicleId)
                    .With(x => x.IsRented, true)
                .Create();

            var user = _fixture.Build<User>()
                    .With(x => x.Id, entity.UserId)
                .Create();
            
            var command = _fixture.Build<UpdateRentCommand>()
                    .With(x => x.Id, entity.Id)
                    .With(x => x.EstimatedDate, startDate.AddDays(numberOfDays))
                .Create();

            return new object[] 
            { 
                new Tuple<string, StatusType>[] { 
                    Tuple.Create(nameof(UpdateRentEvent), StatusType.Success),
                    Tuple.Create(nameof(UpdateRentProjectionEvent), StatusType.Success),
                },
                HttpStatusCode.OK,
                new Entity[]{ entity, user, rentPlane, vehicle },
                command,
                "/api/rent/",
                $"/api/rent/{command.Id.ToString()}"
            };
        })();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
