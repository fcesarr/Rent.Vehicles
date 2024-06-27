using FluentValidation;

using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services.Validators;

public class RentValidator : Validator<Entities.Rent>, IRentValidator
{
    public RentValidator()
    {
        RuleFor(x => x.EstimatedDate)
            .Must((x, y) => y > x.StartDate)
            .WithMessage("Data estimada de termino menor que a data de inicio");
    }
}
