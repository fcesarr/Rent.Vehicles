using System.Net;

using Microsoft.AspNetCore.Mvc;

using Rent.Vehicles.Services.Exceptions;

namespace Rent.Vehicles.Api.Extensions;

public static class ControllerExtension
{
    public static IResult TreatExceptionToResult(this Exception exception,
        HttpContext context)
    {
        ProblemDetailsContext problemDetailsContext = new()
        {
            HttpContext = context,
            Exception = exception,
            ProblemDetails = exception switch
            {
                ValidationException validationException => new ValidationProblemDetails(validationException.GetErros()),
                NullException nullException => new ProblemDetails { Status = (int)HttpStatusCode.NotFound },
                EmptyException emptyException => new ProblemDetails { Status = (int)HttpStatusCode.NoContent },
                _ => new ProblemDetails()
            }
        };

        problemDetailsContext.ProblemDetails.Detail = exception.Message;

        return Results.Problem(problemDetailsContext.ProblemDetails);
    }
}
