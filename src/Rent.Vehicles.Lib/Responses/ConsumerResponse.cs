namespace Rent.Vehicles.Lib.Responses;

public record ConsumerResponse
{
    public required dynamic Id
    {
        get;
        init;
    }

    public required byte[] Data
    {
        get;
        init;
    }
}
