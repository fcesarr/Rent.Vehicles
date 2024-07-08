using System.Collections;
using System.Net;
using System.Reflection;

using AutoFixture;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Types;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Projections.Events;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices.ClassDatas;

public class UpdateUserLicenseImageCommandBackgroundServiceTestData : IEnumerable<object[]>
{
    private readonly Fixture _fixture;

    public UpdateUserLicenseImageCommandBackgroundServiceTestData()
    {
        _fixture = new Fixture();
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new Func<Task<object[]>>(async () => 
        {
            var entity = _fixture
                .Create<User>();

            var command = _fixture.Build<UpdateUserLicenseImageCommand>()
                    .With(x => x.Id, entity.Id)
                    .With(x => x.LicenseImage, await GetBase64StringAsync("pngBase64String"))
                .Create();

            return new object[] 
            { 
                new Tuple<string, StatusType>[] { 
                    Tuple.Create(nameof(UpdateUserLicenseImageEvent), StatusType.Success),
                    Tuple.Create(nameof(UploadUserLicenseImageEvent), StatusType.Success),
                    Tuple.Create(nameof(UpdateUserProjectionEvent), StatusType.Success),
                },
                HttpStatusCode.OK,
                new User[]{entity},
                command,
                "/api/user/upload/licenseImage",
                $"/api/user/{command.Id.ToString()}"
            };
        })().GetAwaiter().GetResult();
        yield return new Func<Task<object[]>>(async () => 
        {
            var entity = _fixture
                .Create<User>();

            var command = _fixture.Build<UpdateUserLicenseImageCommand>()
                    .With(x => x.Id, entity.Id)
                    .With(x => x.LicenseImage, await GetBase64StringAsync("bmpBase64String"))
                .Create();

            return new object[] 
            { 
                new Tuple<string, StatusType>[] { 
                    Tuple.Create(nameof(UpdateUserLicenseImageEvent), StatusType.Success),
                    Tuple.Create(nameof(UploadUserLicenseImageEvent), StatusType.Success),
                    Tuple.Create(nameof(UpdateUserProjectionEvent), StatusType.Success),
                },
                HttpStatusCode.OK,
                new User[]{entity},
                command,
                "/api/user/upload/licenseImage",
                $"/api/user/{command.Id.ToString()}"
            };
        })().GetAwaiter().GetResult();
        yield return new Func<Task<object[]>>(async () => 
        {
            var entity = _fixture
                .Create<User>();

            var command = _fixture.Build<UpdateUserLicenseImageCommand>()
                    .With(x => x.Id, entity.Id)
                    .With(x => x.LicenseImage, await GetBase64StringAsync("jpgBase64String"))
                .Create();

            return new object[] 
            { 
                new Tuple<string, StatusType>[] { 
                    Tuple.Create(nameof(UpdateUserLicenseImageEvent), StatusType.Fail)
                },
                HttpStatusCode.OK,
                new User[]{entity},
                command,
                "/api/user/upload/licenseImage",
                $"/api/user/{command.Id.ToString()}"
            };
        })().GetAwaiter().GetResult();
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
