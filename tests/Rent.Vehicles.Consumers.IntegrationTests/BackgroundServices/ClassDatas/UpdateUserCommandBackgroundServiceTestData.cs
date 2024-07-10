using System.Collections;
using System.Net;

using AutoFixture;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Types;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Projections.Events;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices.ClassDatas;

public class UpdateUserCommandBackgroundServiceTestData : IEnumerable<object[]>
{
    private readonly Fixture _fixture;

    public UpdateUserCommandBackgroundServiceTestData()
    {
        _fixture = new Fixture();
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new Func<object[]>(() =>
        {
            var entity = _fixture
                .Create<User>();

            var command = _fixture.Build<UpdateUserCommand>()
                .With(x => x.Id, entity.Id)
                .Create();

            return new object[]
            {
                new[]
                {
                    Tuple.Create(nameof(UpdateUserEvent), StatusType.Success),
                    Tuple.Create(nameof(UpdateUserProjectionEvent), StatusType.Success)
                },
                HttpStatusCode.OK, new[] { entity }, command, "/api/user/",
                $"/api/user/{command?.Id.ToString()}"
            };
        })();
        yield return new Func<object[]>(() =>
        {
            var entity = _fixture
                .Create<User>();

            var entityHasCreated = _fixture
                .Create<User>();

            var command = _fixture.Build<UpdateUserCommand>()
                .With(x => x.Id, entity.Id)
                .With(x => x.LicenseNumber, entityHasCreated.LicenseNumber)
                .Create();

            return new object[]
            {
                new[] { Tuple.Create(nameof(UpdateUserEvent), StatusType.Fail) }, HttpStatusCode.OK,
                new[] { entity, entityHasCreated }, command, "/api/user/",
                $"/api/user/{entityHasCreated.Id.ToString()}"
            };
        })();
        yield return new Func<object[]>(() =>
        {
            var entity = _fixture
                .Create<User>();

            var entityHasCreated = _fixture
                .Create<User>();

            var command = _fixture.Build<UpdateUserCommand>()
                .With(x => x.Id, entity.Id)
                .With(x => x.Number, entityHasCreated.Number)
                .Create();

            return new object[]
            {
                new[] { Tuple.Create(nameof(UpdateUserEvent), StatusType.Fail) }, HttpStatusCode.OK,
                new[] { entity, entityHasCreated }, command, "/api/user/",
                $"/api/user/{entityHasCreated.Id.ToString()}"
            };
        })();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
