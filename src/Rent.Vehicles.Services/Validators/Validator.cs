using FluentValidation;
using FluentValidation.Results;

using Rent.Vehicles.Services.Validators.Interfaces;

using ValidationException = Rent.Vehicles.Services.Exceptions.ValidationException;

namespace Rent.Vehicles.Services.Validators;

public class Validator<TEntity> : AbstractValidator<TEntity>, Interfaces.IValidator<TEntity> where TEntity : class
{
    async Task<ValidationResult<TEntity>> Interfaces.IValidator<TEntity>.ValidateAsync(TEntity? instance,
        CancellationToken cancellationToken)
    {
        ValidationResult<TEntity> validationResult = new() { IsValid = false };

        if (instance is null)
        {
            validationResult.Exception = new ValidationException($"Error on Validate {typeof(TEntity).Name}",
                new Dictionary<string, string[]>());
            return validationResult;
        }

        ValidationResult? result = await base.ValidateAsync(instance,
            cancellationToken);

        validationResult.IsValid = result.IsValid;

        if (!result.IsValid)
        {
            validationResult.Exception =
                new ValidationException($"Error on Validate {typeof(TEntity).Name}", result.ToDictionary());

            return validationResult;
        }

        validationResult.Instance = instance;

        return validationResult;
    }
}