
using FluentValidation;

using Rent.Vehicles.Services.Validators.Interfaces;

using ValidationException = Rent.Vehicles.Services.Exceptions.ValidationException;

namespace Rent.Vehicles.Services.Validators;

public abstract class Validator<T> : AbstractValidator<T>, Interfaces.IValidator<T>
{
	async Task<ValidationResult> Interfaces.IValidator<T>.ValidateAsync(T instance, CancellationToken cancellationToken)
	{
		var validationResult = new ValidationResult();

		var result = await base.ValidateAsync(instance,
			cancellationToken);

		validationResult.IsValid = result.IsValid;

		if (!result.IsValid)
		{
			validationResult.Exception =
				new ValidationException($"Error on Validate {typeof(T).Name}", result.ToDictionary());
		}

		return validationResult;
	}
}
