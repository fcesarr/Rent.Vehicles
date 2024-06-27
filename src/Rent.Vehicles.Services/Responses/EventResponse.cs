using Rent.Vehicles.Entities.Types;
using Rent.Vehicles.Lib.Types;

namespace Rent.Vehicles.Services.Responses;

public record EventResponse
{
    public required Guid Id
    {
        get;
        init;
    }

    public required Guid SagaId
    {
        get;
        init;
    }

    public required string Name
    {
        get;
        init;
    }

    public required StatusType StatusType
    {
        get;
        init;
    }

    public required string Message
    {
        get;
        init;
    }

    public required SerializerType SerializerType
    {
        get;
        init;
    }

    public required DateTime Created
    {
        get;
        init;
    }
}
