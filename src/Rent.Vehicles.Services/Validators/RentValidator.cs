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
           .Must((e, isActive) => {
                
                if(e.IsActive && isActive)
                    return false;
                    
                return true;
            }).WithMessage("Aluguel já esta ativo")
            .WhenAsync(async (e, cancellationToken) => {
                var entity = await repository.GetAsync(x => (x.Id == e.Id || x.UserId == e.UserId) && x.IsActive,
                    cancellationToken: cancellationToken);
                
                return entity is not null;
            });
        
        RuleFor(x => x.IsActive)
            .Must((e, isActive) => {
                
                if(!e.IsActive && !isActive)
                    return false;
                    
                return true;
            }).WithMessage("Aluguel já está inativo")
            .WhenAsync(async (e, cancellationToken) => {
                var entity = await repository.GetAsync(x => (x.Id == e.Id || x.UserId == e.UserId) && !x.IsActive,
                    cancellationToken: cancellationToken);
                
                return entity is not null;
            });
    }
}
