using Microsoft.AspNetCore.Mvc;

using Rent.Vehicles.Api.Extensions;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Api.Controllers;

[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/[controller]")]
public class EventController : Controller
{
    private readonly IEventFacade _facade;

    public EventController(IEventFacade facade)
    {
        _facade = facade;
    }

    [HttpGet("{sagaId:guid}")]
	[ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(EventResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IResult> GetLicensePlateAsync([FromRoute]Guid sagaId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _facade.FindAsync(x => x.SagaId == sagaId, true, x => x.Created, cancellationToken);

        if(!entities.IsSuccess)
            return entities.Exception!.TreatExceptionToResult(HttpContext);

        return Results.Ok(entities.Value);
    }
}