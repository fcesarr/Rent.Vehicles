using FluentValidation;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services.Validators;

public class UserValidator : Validator<User>, IUserValidator
{
    public UserValidator(IRepository<User> repository)
    {
        RuleFor(x => x.LicenseNumber)
            .MustAsync(async (e, licenseNumber, cancellationToken) => {
                
                var entity = await repository.GetAsync(x => x.Id != e.Id &&
                    x.LicenseNumber == licenseNumber,
                cancellationToken: cancellationToken);
                
                return entity is null;
            }).WithMessage("Numero de licença já cadastrado");

        RuleFor(x => x.Number)
            .MustAsync(async (e, number, cancellationToken) => {
                
                var entity = await repository.GetAsync(x => x.Id != e.Id &&
                    x.Number == number,
                cancellationToken: cancellationToken);
                
                return entity is null;
            }).WithMessage("CNPJ já cadastrado");
    }
}
