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

        RuleFor(x => x.IsActive)
            .MustAsync(async (e, isActive, cancellationToken) => {
                
                var entity = await repository.GetAsync(x => x.Id == e.Id || x.UserId == e.UserId,
                    cancellationToken: cancellationToken);
                
                if(entity is null)
                    return true;

                if(entity.IsActive && isActive)
                    return false;
                    
                return true;
            }).WithMessage("Aluguel ainda ativo");
    }
}
