using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Messages.Types;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Extensions;

public static class EntityExtension
{
    public static RentResponse ToResponse(this Entities.Rent entity, Vehicle vehicle, User user)
    {
        return new RentResponse
        {
            Id = entity.Id,
            NumberOfDays = entity.NumberOfDays,
            DailyCost = entity.DailyCost,
            Vehicle = vehicle.ToResponse(),
            User = user!.ToResponse(),
            StartDate = entity.StartDate,
            PreEndDatePercentageFine = entity.PreEndDatePercentageFine,
            PostEndDateFine = entity.PostEndDateFine,
            EstimatedDate = entity.EstimatedDate,
            EndDate = entity.EndDate,
            Cost = entity.Cost
        };
    }

    public static VehicleResponse ToResponse(this Vehicle entity)
    {
        return new VehicleResponse
        {
            Id = entity.Id,
            Year = entity.Year,
            Model = entity.Model,
            LicensePlate = entity.LicensePlate,
            Type = entity.Type.ToString(),
            IsRented = entity.IsRented
        };
    }

    public static UserResponse ToResponse(this User entity)
    {
        return new UserResponse
        {
            Id = entity.Id,
            Number = entity.Number,
            Name = entity.Name,
            LicenseNumber = entity.LicenseNumber,
            LicenseType = entity.LicenseType,
            LicensePath = entity.LicensePath,
            Birthday = entity.Birthday,
            Created = entity.Created
        };
    }

    public static UserResponse ToResponse(this UserProjection entity)
    {
        return new UserResponse
        {
            Id = entity.Id,
            Number = entity.Number,
            Name = entity.Name,
            LicenseNumber = entity.LicenseNumber,
            LicenseType = entity.LicenseType,
            LicensePath = entity.LicensePath,
            Birthday = entity.Birthday,
            Created = entity.Created
        };
    }

    public static VehicleResponse ToResponse<T>(this T projection) where T : VehicleProjection
    {
        return new VehicleResponse
        {
            Id = projection.Id,
            Year = projection.Year,
            Model = projection.Model,
            LicensePlate = projection.LicensePlate,
            Type = projection.Type.ToString(),
            IsRented = projection.IsRented
        };
    }

    public static User ToEntity(this CreateUserEvent @event, string licensePath)
    {
        return new User
        {
            Id = @event.Id,
            Name = @event.Name,
            Number = @event.Number,
            Birthday = @event.Birthday,
            LicenseNumber = @event.LicenseNumber,
            LicenseType = @event.LicenseType.TreatType(),
            LicensePath = licensePath
        };
    }

    public static User ToEntity(this UpdateUserEvent @event, User entity)
    {
        entity.Name = @event.Name;
        entity.Number = @event.Number;
        entity.Birthday = @event.Birthday;
        entity.LicenseNumber = @event.LicenseNumber;
        entity.LicenseType = @event.LicenseType.TreatType();

        return entity;
    }

    public static Vehicle ToEntity(this CreateVehiclesEvent @event)
    {
        return new Vehicle
        {
            Id = @event.Id,
            Year = @event.Year,
            Model = @event.Model,
            LicensePlate = @event.LicensePlate,
            Type = @event.Type switch
            {
                VehicleType.B => Entities.Types.VehicleType.B,
                VehicleType.C => Entities.Types.VehicleType.C,
                VehicleType.D => Entities.Types.VehicleType.D,
                VehicleType.E => Entities.Types.VehicleType.E,
                VehicleType.A or _ => Entities.Types.VehicleType.A
            }
        };
    }

    public static T ToProjection<T>(this Vehicle entity) where T : VehicleProjection, new()
    {
        return new T
        {
            Id = entity.Id,
            Year = entity.Year,
            Model = entity.Model,
            LicensePlate = entity.LicensePlate,
            Type = entity.Type
        };
    }

    private static Entities.Types.LicenseType TreatType(this Messages.Types.LicenseType type)
    {
        return type switch
        {
            LicenseType.B => Entities.Types.LicenseType.B,
            LicenseType.AB => Entities.Types.LicenseType.AB,
            LicenseType.A or _ => Entities.Types.LicenseType.A
        };
    }

}
