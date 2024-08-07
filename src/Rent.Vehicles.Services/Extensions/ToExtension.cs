using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Entities.Types;
using Rent.Vehicles.Lib.Types;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Services.Responses;

using Event = Rent.Vehicles.Entities.Event;
using StatusType = Rent.Vehicles.Messages.Types.StatusType;
using VehicleType = Rent.Vehicles.Messages.Types.VehicleType;

namespace Rent.Vehicles.Services.Extensions;

public static class ToExtension
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

    public static EventResponse ToResponse(this Event entity)
    {
        return new EventResponse
        {
            Id = entity.Id,
            SagaId = entity.SagaId,
            StatusType = entity.StatusType,
            SerializerType = entity.SerializerType,
            Name = entity.Name,
            Message = entity.Message,
            Created = entity.Created
        };
    }

    public static CommandResponse ToResponse(this Command entity)
    {
        return new CommandResponse
        {
            Id = entity.Id,
            SagaId = entity.SagaId,
            ActionType = entity.ActionType,
            SerializerType = entity.SerializerType,
            EntityType = entity.EntityType,
            Type = entity.Type
        };
    }

    public static RentalPlaneResponse ToResponse(this RentalPlane entity)
    {
        return new RentalPlaneResponse
        {
            Id = entity.Id,
            NumberOfDays = entity.NumberOfDays,
            DailyCost = entity.DailyCost,
            PreEndDatePercentageFine = entity.PreEndDatePercentageFine,
            PostEndDateFine = entity.PostEndDateFine
        };
    }

    public static RentResponse ToResponse(this RentProjection projection)
    {
        return new RentResponse
        {
            Id = projection.Id,
            NumberOfDays = projection.NumberOfDays,
            DailyCost = projection.DailyCost,
            StartDate = projection.StartDate,
            EndDate = projection.EndDate,
            EstimatedDate = projection.EstimatedDate,
            PreEndDatePercentageFine = projection.PreEndDatePercentageFine,
            PostEndDateFine = projection.PostEndDateFine,
            Cost = projection.Cost,
            Vehicle = projection.Vehicle!.ToResponse(),
            User = projection.User!.ToResponse()
        };
    }

    public static EventResponse ToResponse(this EventProjection projection)
    {
        return new EventResponse
        {
            Id = projection.Id,
            SagaId = projection.SagaId,
            Name = projection.Name,
            StatusType = projection.StatusType,
            Message = projection.Message,
            SerializerType = projection.SerializerType,
            Created = projection.Created
        };
    }

    public static User ToEntity(this CreateUserEvent @event, string licensePath)
    {
        return new User
        {
            Id = @event.Id,
            Name = @event.Name,
            Number = @event.Number,
            Birthday = DateTime.SpecifyKind(@event.Birthday, DateTimeKind.Local),
            LicenseNumber = @event.LicenseNumber,
            LicenseType = TreatType(@event.LicenseType),
            LicensePath = licensePath
        };
    }

    public static User ToEntity(this UpdateUserEvent @event, User entity)
    {
        entity.Name = @event.Name ?? entity.Name;
        entity.Number = @event.Number ?? entity.Number;
        entity.Birthday = DateTime.SpecifyKind(@event.Birthday ?? entity.Birthday, DateTimeKind.Local);
        entity.LicenseNumber = @event.LicenseNumber ?? entity.LicenseNumber;
        entity.LicenseType = TreatType(@event.LicenseType);

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

    public static Event ToEntity(this Messages.Events.Event @event,
        IEnumerable<byte> data)
    {
        return new Event
        {
            Id = @event.Id,
            SagaId = @event.SagaId,
            Name = @event.Type,
            StatusType = @event.StatusType switch
            {
                StatusType.Success => Entities.Types.StatusType.Success,
                StatusType.Fail or _ => Entities.Types.StatusType.Fail
            },
            Message = @event.Message,
            SerializerType = SerializerType.MessagePack,
            Data = data.ToList()
        };
    }

    public static Command ToEntity(this Lib.Command command,
        ActionType actionType,
        SerializerType serializerType,
        EntityType entityType,
        string type,
        IEnumerable<byte> data)
    {
        return new Command
        {
            SagaId = command.SagaId,
            ActionType = actionType,
            SerializerType = serializerType,
            EntityType = entityType,
            Type = type,
            Data = data.ToList()
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

    public static VehicleProjection ToProjection(this Vehicle entity)
    {
        return entity.ToProjection<VehicleProjection>();
    }


    public static UserProjection ToProjection(this User entity)
    {
        return new UserProjection
        {
            Id = entity.Id,
            Name = entity.Name,
            Number = entity.Number,
            Birthday = entity.Birthday,
            LicenseNumber = entity.LicenseNumber,
            LicensePath = entity.LicenseNumber,
            LicenseType = entity.LicenseType
        };
    }

    public static RentProjection ToProjection(this Entities.Rent entity)
    {
        return new RentProjection
        {
            Id = entity.Id,
            NumberOfDays = entity.NumberOfDays,
            DailyCost = entity.DailyCost,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            EstimatedDate = entity.EstimatedDate,
            PreEndDatePercentageFine = entity.PreEndDatePercentageFine,
            PostEndDateFine = entity.PostEndDateFine,
            Cost = entity.Cost,
            Vehicle = entity.Vehicle!.ToProjection<VehicleProjection>(),
            User = entity.User!.ToProjection()
        };
    }

    public static EventProjection ToProjection(this Event entity)
    {
        return new EventProjection
        {
            Id = entity.Id,
            SagaId = entity.SagaId,
            Name = entity.Name,
            StatusType = entity.StatusType,
            Message = entity.Message,
            SerializerType = entity.SerializerType
        };
    }

    private static LicenseType TreatType(Messages.Types.LicenseType? type)
    {
        return type switch
        {
            Messages.Types.LicenseType.B => LicenseType.B,
            Messages.Types.LicenseType.AB => LicenseType.AB,
            Messages.Types.LicenseType.A or _ => LicenseType.A
        };
    }
}
