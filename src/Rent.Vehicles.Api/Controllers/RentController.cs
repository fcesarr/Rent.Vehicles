using Microsoft.AspNetCore.Mvc;

using Rent.Vehicles.Api.Responses;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.Facades.Interfaces;

namespace Rent.Vehicles.Api.Controllers;

[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/[controller]")]
public class RentController : Controller
{
    private readonly IPublisher _publisher;

    private readonly IRentFacade _rentFacade;

    public RentController(IPublisher publisher, IRentFacade rentFacade)
    {
        _publisher = publisher;
        _rentFacade = rentFacade;
    }

    [HttpPost]
	[ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(CommandResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IResult> PostAsync([FromBody]CreateRentCommand command,
        CancellationToken cancellationToken = default)
    {
        command.SagaId = Guid.NewGuid();

        await _publisher.PublishCommandAsync(command, cancellationToken);

        string locationUri = $"/Events/status/{command.SagaId}";

        return Results.Accepted(locationUri, command);
    }
}