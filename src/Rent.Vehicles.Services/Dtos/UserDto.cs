using Rent.Vehicles.Entities.Types;

namespace Rent.Vehicles.Services.Dtos;

public record UserDto
{
    public required Guid Id { get; init; }

    public required string Name { get; set; }

    public required string Number { get; set; }

    public required DateTime Birthday { get; set; }

    public required string LicenseNumber { get; set; }

    public LicenseType LicenseType { get; set; }

    public required string LicenseImage { get; set; }
}