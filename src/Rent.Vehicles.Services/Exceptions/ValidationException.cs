using System.Diagnostics.CodeAnalysis;

namespace Rent.Vehicles.Services.Exceptions;

[ExcludeFromCodeCoverage]
public class ValidationException : NoRetryException
{
	private readonly IDictionary<string, string[]> _erros;

	public ValidationException(string message, IDictionary<string, string[]> erros) : base(message)
	{
		_erros = erros;
	}

	public IDictionary<string, string[]> GetErros()
	{
		return _erros;
	}
}
