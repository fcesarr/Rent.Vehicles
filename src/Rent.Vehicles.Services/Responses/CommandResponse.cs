using Rent.Vehicles.Entities.Types;
using Rent.Vehicles.Lib.Types;

namespace Rent.Vehicles.Services.Responses;

public record CommandResponse
{
    public required Guid Id
    {
        get;
        set;
    }

    public required Guid SagaId
    {
        get;
        set;
    }

    public required ActionType ActionType
    {
        get;
        set;
    }

    public required SerializerType SerializerType
    {
        get;
        set;
    }

    public required EntityType EntityType
    {
        get;
        set;
    }

    public required string Type
    {
        get;
        set;
    }
}
