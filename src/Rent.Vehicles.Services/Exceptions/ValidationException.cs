using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Rent.Vehicles.Services.Exceptions;

[ExcludeFromCodeCoverage]
public class ValidationException : NoRetryException
{
    private readonly IDictionary<string, string[]> _erros;

    public ValidationException(string message, IDictionary<string, string[]> erros) : base($"{message}\n{FormatErrors(erros)}")
    {
        _erros = erros;
    }

    public IDictionary<string, string[]> GetErros()
    {
        return _erros;
    }

    private static string FormatErrors(IDictionary<string, string[]> erros)
    {
        var errorMessages = new StringBuilder();
        foreach (var erro in erros)
        {
            errorMessages.AppendLine($"{erro.Key}: {string.Join(", ", erro.Value)}");
        }
        return errorMessages.ToString();
    }
}
