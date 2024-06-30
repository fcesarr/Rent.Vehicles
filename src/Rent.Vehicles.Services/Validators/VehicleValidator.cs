using FluentValidation;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services.Validators;

public class VehicleValidator : Validator<Vehicle>, IVehicleValidator
{
    public VehicleValidator(IRepository<Vehicle> repository)
    {
        RuleFor(x => x.LicensePlate)
            .MustAsync(async (licensePlate, cancellationToken) => {
                
                var entity = await repository.GetAsync(x => x.LicensePlate == licensePlate,
                    cancellationToken: cancellationToken);
                
                return entity is null;
            }).WithMessage("Placa já cadastrada");
    }
}
