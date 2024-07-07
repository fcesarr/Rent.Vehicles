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
            .MustAsync(async (e, licensePlate, cancellationToken) => {
                
                var entity = await repository.GetAsync(x => x.Id != e.Id &&
                    x.LicensePlate == licensePlate,
                cancellationToken: cancellationToken);
                
                return entity is null;
            }).WithMessage("Veiculo com placa já cadastrada");

        RuleFor(x => x)
            .MustAsync(async (e, cancellationToken) => {
                
                var entity = await repository.GetAsync(x => x.Id == e.Id &&
                    x.IsRented,
                cancellationToken: cancellationToken);
                
                return entity is null;
            }).WithMessage("Veiculo já alugado");
    }
}
