namespace Rent.Vehicles.Consumers.Responses;

public record ConsumerResponse
{
    public required dynamic Id { get; init; }

    public required byte[] Data { get; init; }
}