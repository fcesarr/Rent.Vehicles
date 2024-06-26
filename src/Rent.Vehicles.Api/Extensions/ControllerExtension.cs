using System.Net;

using Microsoft.AspNetCore.Mvc;

using Rent.Vehicles.Services.Exceptions;

namespace Rent.Vehicles.Api.Extensions;

public static class ControllerExtension
{
    public static IResult TreatExceptionToResult(this Exception exception,
        HttpContext context)
    {
        var problemDetailsContext = new ProblemDetailsContext
        {
            HttpContext = context,
            Exception = exception,
            ProblemDetails = exception switch
            {
                ValidationException validationException => new ValidationProblemDetails(validationException.GetErros())
                {
                    Detail = exception.Message
                },
                NullException nullException => new ProblemDetails
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Detail = nullException.Message
                },
                EmptyException emptyException => new ProblemDetails
                {
                    Status = (int)HttpStatusCode.NoContent,
                    Detail = emptyException.Message
                },
                _ => new ProblemDetails
                {
                    Detail = exception.Message
                }
            }
        };

        return Results.Problem(problemDetailsContext.ProblemDetails);
    }
}