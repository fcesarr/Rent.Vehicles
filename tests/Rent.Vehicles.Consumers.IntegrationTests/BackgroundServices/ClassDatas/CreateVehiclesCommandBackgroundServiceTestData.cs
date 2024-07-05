using System.Collections;
using System.Net;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Types;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Projections.Events;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices.ClassDatas;

public class CreateVehiclesCommandBackgroundServiceTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] 
        { 
            2024,
            new Tuple<string, StatusType>[] { 
                Tuple.Create(nameof(CreateVehiclesEvent), StatusType.Success),
                Tuple.Create(nameof(CreateVehiclesForSpecificYearEvent), StatusType.Success),
                Tuple.Create(nameof(CreateVehiclesProjectionEvent), StatusType.Success),
                Tuple.Create(nameof(CreateVehiclesForSpecificYearProjectionEvent), StatusType.Success)
            },
            HttpStatusCode.OK,
            false,
        };
        yield return new object[] 
        { 
            2025,
            new Tuple<string, StatusType>[] { 
                Tuple.Create(nameof(CreateVehiclesEvent), StatusType.Success),
                Tuple.Create(nameof(CreateVehiclesForSpecificYearEvent), StatusType.Fail),
                Tuple.Create(nameof(CreateVehiclesProjectionEvent), StatusType.Success)
            },
            HttpStatusCode.OK,
            false,
        };
        yield return new object[] 
        { 
            2025,
            new Tuple<string, StatusType>[] { 
                Tuple.Create(nameof(CreateVehiclesEvent), StatusType.Fail)
            },
            HttpStatusCode.NotFound,
            true,
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
