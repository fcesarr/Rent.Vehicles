using Rent.Vehicles.Services.Exceptions;

namespace Rent.Vehicles.Services.Validators.Interfaces;

public interface IValidator<T>
{
	Task<ValidationResult> ValidateAsync(T instance,
		CancellationToken cancellationToken = default);
}

public class ValidationResult
{
	public bool IsValid { get; set; }

	public ValidationException? Exception { get; set; }
}