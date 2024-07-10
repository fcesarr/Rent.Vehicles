using FluentValidation;

using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services.Validators;

public class RentValidator : Validator<Entities.Rent>, IRentValidator
{
    public RentValidator(IRepository<Entities.Rent> repository)
    {
        RuleFor(x => x.EstimatedDate)
            .Must((e, estimatedDate) => estimatedDate > e.StartDate)
            .WithMessage("Data estimada de termino menor que a data de inicio");

        RuleFor(x => x)
            .CustomAsync(async (e, context, cancellationToken) =>
            {
                var entity = await repository.GetAsync(x => x.UserId == e.UserId
                    , true
                    , x => x.Created
                    , cancellationToken: cancellationToken);


                if (entity is Entities.Rent { IsActive: true, Updated: var updatedActive } &&
                    updatedActive == default && e.IsActive)
                {
                    context.AddFailure("IsActive", "Aluguel já está ativo");
                    return;
                }

                if (entity is Entities.Rent { IsActive: false, Updated: var updatedInactive } &&
                    updatedInactive != default && !e.IsActive)
                {
                    context.AddFailure("IsActive", "Aluguel já está inativo");
                    return;
                }
            });
    }
}
