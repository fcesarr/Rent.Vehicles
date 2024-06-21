using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Exceptions;

namespace Rent.Vehicles.Services.Validators.Interfaces;

public interface IValidator<TEntity> where TEntity : class
{
	Task<ValidationResult<TEntity>> ValidateAsync(TEntity? instance,
		CancellationToken cancellationToken = default);
}

public class ValidationResult<T> where T : class
{
	public bool IsValid { get; set; }

    public T Instance { get; set; } = default!;

	public ValidationException? Exception { get; set; }
}