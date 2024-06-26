using FluentValidation;

using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services.Validators;

public class RentValidator : Validator<Entities.Rent>, IRentValidator
{
    public RentValidator()
    {
        RuleFor(x => x.EndDate)
            .Must((x, y) => x.StartDate > y)
            .WithMessage("Data estimada de termino menor que a data de inicio");
    }
}