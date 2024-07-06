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

public class CreateUserCommandBackgroundServiceTestData : IEnumerable<object[]>
{
    private readonly Fixture _fixture;

    public CreateUserCommandBackgroundServiceTestData()
    {
        _fixture = new Fixture();
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new Func<Task<object[]>>(async () => 
        {
            var command = _fixture.Build<CreateUserCommand>()
                    .Without(x => x.Id)
                    .With(x => x.LicenseImage, await GetBase64StringAsync("pngBase64String"))
                .Create();

            return new object[] 
            { 
                new Tuple<string, StatusType>[] { 
                    Tuple.Create(nameof(CreateUserEvent), StatusType.Success),
                    Tuple.Create(nameof(UploadUserLicenseImageEvent), StatusType.Success),
                    Tuple.Create(nameof(CreateUserProjectionEvent), StatusType.Success),
                },
                HttpStatusCode.OK,
                Array.Empty<User>(),
                command
            };
        })().GetAwaiter().GetResult();
        yield return new Func<Task<object[]>>(async () => {

            var command = _fixture.Build<CreateUserCommand>()
                    .Without(x => x.Id)
                    .With(x => x.LicenseImage, await GetBase64StringAsync("bmpBase64String"))
                .Create();

            return new object[] 
            { 
                new Tuple<string, StatusType>[] { 
                    Tuple.Create(nameof(CreateUserEvent), StatusType.Success),
                    Tuple.Create(nameof(UploadUserLicenseImageEvent), StatusType.Success),
                    Tuple.Create(nameof(CreateUserProjectionEvent), StatusType.Success),
                },
                HttpStatusCode.OK,
                Array.Empty<User>(),
                command
            };
        })().GetAwaiter().GetResult();
        yield return new Func<Task<object[]>>(async () => {

            var command = _fixture.Build<CreateUserCommand>()
                    .Without(x => x.Id)
                    .With(x => x.LicenseImage, await GetBase64StringAsync("jpgBase64String"))
                .Create();

            return new object[] 
            { 
                new Tuple<string, StatusType>[] { 
                    Tuple.Create(nameof(CreateUserEvent), StatusType.Fail)
                },
                HttpStatusCode.NotFound,
                Array.Empty<User>(),
                command
            };
        })().GetAwaiter().GetResult();
        yield return new Func<Task<object[]>>(async () => {
            var entity = _fixture.Create<User>();

            var command = _fixture.Build<CreateUserCommand>()
                    .Without(x => x.Id)
                    .With(x => x.LicenseImage, await GetBase64StringAsync("pngBase64String"))
                    .With(x => x.Number, entity.Number)
                .Create();

            return new object[] 
            { 
                new Tuple<string, StatusType>[] { 
                    Tuple.Create(nameof(CreateUserEvent), StatusType.Fail)
                },
                HttpStatusCode.NotFound,
                new User[]{entity},
                command
            };
        })().GetAwaiter().GetResult();
        yield return new Func<Task<object[]>>(async () => {
            var entity = _fixture.Create<User>();

            var command = _fixture.Build<CreateUserCommand>()
                    .Without(x => x.Id)
                    .With(x => x.LicenseImage, await GetBase64StringAsync("bmpBase64String"))
                    .With(x => x.Number, entity.Number)
                .Create();

            return new object[] 
            { 
                new Tuple<string, StatusType>[] { 
                    Tuple.Create(nameof(CreateUserEvent), StatusType.Fail)
                },
                HttpStatusCode.NotFound,
                new User[]{entity},
                command
            };
        })().GetAwaiter().GetResult();
        yield return new Func<Task<object[]>>(async () => {
            var entity = _fixture.Create<User>();

            var command = _fixture.Build<CreateUserCommand>()
                    .Without(x => x.Id)
                    .With(x => x.LicenseImage, await GetBase64StringAsync("pngBase64String"))
                    .With(x => x.LicenseNumber, entity.LicenseNumber)
                .Create();

            return new object[] 
            { 
                new Tuple<string, StatusType>[] { 
                    Tuple.Create(nameof(CreateUserEvent), StatusType.Fail)
                },
                HttpStatusCode.NotFound,
                new User[]{entity},
                command
            };
        })().GetAwaiter().GetResult();
        yield return new Func<Task<object[]>>(async () => {
            var entity = _fixture.Create<User>();

            var command = _fixture.Build<CreateUserCommand>()
                    .Without(x => x.Id)
                    .With(x => x.LicenseImage, await GetBase64StringAsync("bmpBase64String"))
                    .With(x => x.LicenseNumber, entity.LicenseNumber)
                .Create();

            return new object[] 
            { 
                new Tuple<string, StatusType>[] { 
                    Tuple.Create(nameof(CreateUserEvent), StatusType.Fail)
                },
                HttpStatusCode.NotFound,
                new User[]{entity},
                command
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
