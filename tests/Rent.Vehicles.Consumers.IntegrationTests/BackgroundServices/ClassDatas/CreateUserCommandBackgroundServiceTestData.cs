using System.Collections;
using System.Net;
using System.Reflection;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Types;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Projections.Events;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices.ClassDatas;

public class CreateUserCommandBackgroundServiceTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] 
        { 
            new Tuple<string, StatusType>[] { 
                Tuple.Create(nameof(CreateUserEvent), StatusType.Success),
                Tuple.Create(nameof(UploadUserLicenseImageEvent), StatusType.Success),
                Tuple.Create(nameof(CreateUserProjectionEvent), StatusType.Success),
            },
            HttpStatusCode.OK,
            GetBase64StringAsync("pngBase64String")
                .GetAwaiter()
                .GetResult(),
            false,
            false,
            false,
        };
        yield return new object[] 
        { 
            new Tuple<string, StatusType>[] { 
                Tuple.Create(nameof(CreateUserEvent), StatusType.Fail),
            },
            HttpStatusCode.NotFound,
            GetBase64StringAsync("pngBase64String")
                .GetAwaiter()
                .GetResult(),
            true,
            false,
            true
        };
        yield return new object[] 
        { 
            new Tuple<string, StatusType>[] { 
                Tuple.Create(nameof(CreateUserEvent), StatusType.Fail),
            },
            HttpStatusCode.NotFound,
            GetBase64StringAsync("pngBase64String")
                .GetAwaiter()
                .GetResult(),
            true,
            true,
            false
        };
        yield return new object[] 
        { 
            new Tuple<string, StatusType>[] { 
                Tuple.Create(nameof(CreateUserEvent), StatusType.Success),
                Tuple.Create(nameof(UploadUserLicenseImageEvent), StatusType.Success),
                Tuple.Create(nameof(CreateUserProjectionEvent), StatusType.Success),
            },
            HttpStatusCode.OK,
            GetBase64StringAsync("bmpBase64String")
                .GetAwaiter()
                .GetResult(),
            false,
            false,
            false,
        };
        yield return new object[] 
        { 
            new Tuple<string, StatusType>[] { 
                Tuple.Create(nameof(CreateUserEvent), StatusType.Fail),
            },
            HttpStatusCode.NotFound,
            GetBase64StringAsync("bmpBase64String")
                .GetAwaiter()
                .GetResult(),
            true,
            false,
            true
        };
        yield return new object[] 
        { 
            new Tuple<string, StatusType>[] { 
                Tuple.Create(nameof(CreateUserEvent), StatusType.Fail),
            },
            HttpStatusCode.NotFound,
            GetBase64StringAsync("bmpBase64String")
                .GetAwaiter()
                .GetResult(),
            true,
            true,
            false
        };
        yield return new object[] 
        { 
            new Tuple<string, StatusType>[] { 
                Tuple.Create(nameof(CreateUserEvent), StatusType.Fail),
            },
            HttpStatusCode.NotFound,
            GetBase64StringAsync("jpgBase64String")
                .GetAwaiter()
                .GetResult(),
            false,
            false,
            false,
        };
        yield return new object[] 
        { 
            new Tuple<string, StatusType>[] { 
                Tuple.Create(nameof(CreateUserEvent), StatusType.Fail),
            },
            HttpStatusCode.NotFound,
            GetBase64StringAsync("jpgBase64String")
                .GetAwaiter()
                .GetResult(),
            true,
            false,
            true
        };
        yield return new object[] 
        { 
            new Tuple<string, StatusType>[] { 
                Tuple.Create(nameof(CreateUserEvent), StatusType.Fail),
            },
            HttpStatusCode.NotFound,
            GetBase64StringAsync("jpgBase64String")
                .GetAwaiter()
                .GetResult(),
            true,
            true,
            false
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private static async Task<string> GetBase64StringAsync(string name, CancellationToken cancellationToken = default)
    {
        name = $"Rent.Vehicles.Consumers.IntegrationTests.Images.{name}";

        var result = string.Empty;

        var assembly = Assembly.GetExecutingAssembly();

        if(assembly.GetManifestResourceStream(name) is Stream stream)
        {
            using StreamReader reader = new StreamReader(stream);
            result = await reader.ReadToEndAsync(cancellationToken);
            await stream.DisposeAsync();
        }

        return result;
    }
}
