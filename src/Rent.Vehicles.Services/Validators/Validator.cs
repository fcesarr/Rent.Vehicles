
using FluentValidation;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Validators.Interfaces;

using ValidationException = Rent.Vehicles.Services.Exceptions.ValidationException;

namespace Rent.Vehicles.Services.Validators;

public class Validator<TEntity> : AbstractValidator<TEntity>, Interfaces.IValidator<TEntity> where TEntity : Entity
{
	async Task<ValidationResult<TEntity>> Interfaces.IValidator<TEntity>.ValidateAsync(TEntity? instance, CancellationToken cancellationToken)
	{
		var validationResult = new ValidationResult<TEntity>
        {
            IsValid = false
        };

        if(instance is null)
        {
            validationResult.Exception = new ValidationException($"Error on Validate {typeof(TEntity).Name}",
                new Dictionary<string, string[]>());
            return validationResult;
        }

		var result = await base.ValidateAsync(instance,
			cancellationToken);

		validationResult.IsValid = result.IsValid;

		if (!result.IsValid)
		{
			validationResult.Exception =
				new ValidationException($"Error on Validate {typeof(TEntity).Name}", result.ToDictionary());
		}

        validationResult.Instance = instance;

		return validationResult;
	}
}
