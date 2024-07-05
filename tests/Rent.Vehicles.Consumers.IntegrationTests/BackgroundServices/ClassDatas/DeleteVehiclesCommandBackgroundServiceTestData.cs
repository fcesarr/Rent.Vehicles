using System.Collections;
using System.Net;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Types;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Projections.Events;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices.ClassDatas;

public class DeleteVehiclesCommandBackgroundServiceTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] 
        { 
            new Tuple<string, StatusType>[] { 
                Tuple.Create(nameof(DeleteVehiclesEvent), StatusType.Success),
                Tuple.Create(nameof(DeleteVehiclesProjectionEvent), StatusType.Success),
            },
            HttpStatusCode.NotFound,
            false
        };
        yield return new object[] 
        { 
            new Tuple<string, StatusType>[] { 
                Tuple.Create(nameof(DeleteVehiclesEvent), StatusType.Fail),
            },
            HttpStatusCode.OK,
            true
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
