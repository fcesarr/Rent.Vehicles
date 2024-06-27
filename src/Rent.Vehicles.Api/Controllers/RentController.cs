using Microsoft.AspNetCore.Mvc;

using Rent.Vehicles.Api.Extensions;
using Rent.Vehicles.Api.Responses;
using Rent.Vehicles.Lib.Attributes;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Responses;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Api.Controllers;

[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/[controller]")]
public class RentController : Controller
{
    private readonly IValidator<CreateRentCommand> _createCommandValidator;
    private readonly IPublisher _publisher;

    private readonly IRentFacade _rentFacade;

    private readonly IValidator<UpdateRentCommand> _updateCommandValidator;

    public RentController(IPublisher publisher,
        IValidator<CreateRentCommand> createRentCommandValidator,
        IValidator<UpdateRentCommand> updateRentCommandValidator,
        IRentFacade rentFacade)
    {
        _publisher = publisher;
        _createCommandValidator = createRentCommandValidator;
        _updateCommandValidator = updateRentCommandValidator;
        _rentFacade = rentFacade;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(CommandResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IResult> PostAsync([FromBody] CreateRentCommand command,
        CancellationToken cancellationToken = default)
    {
        command.SagaId = Guid.NewGuid();

        ValidationResult<CreateRentCommand> result = await _createCommandValidator
            .ValidateAsync(command, cancellationToken);

        if (!result.IsValid)
        {
            return result.Exception.TreatExceptionToResult(HttpContext);
        }

        await _publisher.PublishCommandAsync(command, cancellationToken);

        string locationUri = $"/Events/status/{command.SagaId}";

        return Results.Accepted(locationUri, new CommandResponse(command.Id));
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(CommandResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IResult> PutAsync([FromBody] UpdateRentCommand command,
        CancellationToken cancellationToken = default)
    {
        command.SagaId = Guid.NewGuid();

        ValidationResult<UpdateRentCommand> result = await _updateCommandValidator
            .ValidateAsync(command, cancellationToken);

        if (!result.IsValid)
        {
            return result.Exception.TreatExceptionToResult(HttpContext);
        }

        await _publisher.PublishCommandAsync(command, cancellationToken);

        string locationUri = $"/Events/status/{command.SagaId}";

        return Results.Accepted(locationUri, new CommandResponse(command.Id));
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
        Result<CostResponse> cost = await _rentFacade.EstimateCostAsync(id, estimatedDate, cancellationToken);

        if (!cost.IsSuccess)
        {
            return cost.Exception!.TreatExceptionToResult(HttpContext);
        }

        return Results.Ok(cost.Value);
    }
}