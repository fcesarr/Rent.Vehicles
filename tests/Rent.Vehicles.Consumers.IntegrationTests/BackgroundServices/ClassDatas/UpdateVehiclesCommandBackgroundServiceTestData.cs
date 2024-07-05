using System.Collections;
using System.Net;

using AutoFixture;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Types;
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
        yield return new object[] 
        { 
            new Tuple<string, StatusType>[] { 
                Tuple.Create(nameof(UpdateVehiclesEvent), StatusType.Success),
                Tuple.Create(nameof(UpdateVehiclesProjectionEvent), StatusType.Success),
            },
            HttpStatusCode.OK,
            _fixture.Build<Vehicle>()
                .With(x => x.IsRented, false)    
            .Create(),
        };
        yield return new object[] 
        { 
            new Tuple<string, StatusType>[] { 
                Tuple.Create(nameof(UpdateVehiclesEvent), StatusType.Fail),
            },
            HttpStatusCode.OK,
            _fixture.Build<Vehicle>()
                .With(x => x.IsRented, true)    
            .Create(),
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
