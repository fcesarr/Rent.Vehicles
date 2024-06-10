namespace Rent.Vehicles.Entities;

public class Entity
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTime Created { get; } = DateTime.UtcNow;
}