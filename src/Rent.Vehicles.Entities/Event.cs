namespace Rent.Vehicles.Entities;

public class Event : Entity
{
    public required Guid SagaId { get; set; }

    public required string Name { get; set; }

    public required StatusType StatusType { get; set; }

    public required string Message { get; set; }
}

public enum StatusType
{
    Success,
    Fail
}