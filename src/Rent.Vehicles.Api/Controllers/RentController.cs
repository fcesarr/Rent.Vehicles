using Microsoft.AspNetCore.Mvc;

using Rent.Vehicles.Api.Extensions;
using Rent.Vehicles.Lib.Attributes;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Responses;
using Rent.Vehicles.Services.Validators.Interfaces;

using CommandResponse = Rent.Vehicles.Api.Responses.CommandResponse;

namespace Rent.Vehicles.Api.Controllers;

[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/[controller]")]
public class RentController : Controller
{
    private readonly IValidator<CreateRentCommand> _createCommandValidator;
    private readonly IPublisher _publisher;
    private readonly IRentProjectionFacade _rentProjectionFacade;
    private readonly IValidator<UpdateRentCommand> _updateCommandValidator;
    private readonly Func<Guid, string> GetLocationUri = (Guid sagaId) =>  $"api/event/{sagaId.ToString()}";

    public RentController(IPublisher publisher,
        IValidator<CreateRentCommand> createRentCommandValidator,
        IValidator<UpdateRentCommand> updateRentCommandValidator,
        IRentProjectionFacade rentProjectionFacade)
    {
        _publisher = publisher;
        _createCommandValidator = createRentCommandValidator;
        _updateCommandValidator = updateRentCommandValidator;
        _rentProjectionFacade = rentProjectionFacade;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(CommandResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IResult> PostAsync([FromBody] CreateRentCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _createCommandValidator
            .ValidateAsync(command, cancellationToken);

        if (!result.IsValid)
        {
            return result.Exception.TreatExceptionToResult(HttpContext);
        }

        await _publisher.PublishCommandAsync(command, cancellationToken);

        return Results.Accepted(GetLocationUri(command.SagaId), new CommandResponse(command.Id));
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(CommandResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IResult> PutAsync([FromBody] UpdateRentCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _updateCommandValidator
            .ValidateAsync(command, cancellationToken);

        if (!result.IsValid)
        {
            return result.Exception.TreatExceptionToResult(HttpContext);
        }

        await _publisher.PublishCommandAsync(command, cancellationToken);

        return Results.Accepted(GetLocationUri(command.SagaId), new CommandResponse(command.Id));
    }

    [HttpGet("cost/{id:guid}/{estimatedDate:datetime}")]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(CostResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IResult> GetAsync([FromRoute] Guid id,
        [FromRoute] [DateTimeMinorCurrentDate(ErrorMessage = "Invalid date")]
        DateTime estimatedDate,
        CancellationToken cancellationToken = default)
    {
        var cost = await _rentProjectionFacade.EstimateCostAsync(id, estimatedDate, cancellationToken);

        if (!cost.IsSuccess)
        {
            return cost.Exception!.TreatExceptionToResult(HttpContext);
        }

        return Results.Ok(cost.Value);
    }
}
