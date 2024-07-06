using Microsoft.AspNetCore.Mvc;

using Rent.Vehicles.Api.Extensions;
using Rent.Vehicles.Api.Responses;
using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;
using Rent.Vehicles.Services.Responses;

using CommandResponse = Rent.Vehicles.Api.Responses.CommandResponse;

namespace Rent.Vehicles.Api.Controllers;

[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/[controller]")]
public class VehicleController : Controller
{
    private readonly IValidator<CreateVehiclesCommand> _createCommandValidator;
    private readonly IVehicleProjectionFacade _vehicleProjectionFacade;
    private readonly IValidator<DeleteVehiclesCommand> _deleteCommandValidator;
    private readonly IPublisher _publisher;
    private readonly IValidator<UpdateVehiclesCommand> _updateCommandValidator;
    private readonly Func<Guid, string> GetLocationUri = (Guid sagaId) =>  $"api/event/{sagaId.ToString()}";

    public VehicleController(IPublisher publisher, IValidator<CreateVehiclesCommand> createCommandValidator,
        IValidator<UpdateVehiclesCommand> updateCommandValidator,
        IValidator<DeleteVehiclesCommand> deleteCommandValidator, IVehicleProjectionFacade vehicleProjectionFacade)
    {
        _publisher = publisher;
        _createCommandValidator = createCommandValidator;
        _updateCommandValidator = updateCommandValidator;
        _deleteCommandValidator = deleteCommandValidator;
        _vehicleProjectionFacade = vehicleProjectionFacade;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(CommandResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IResult> PostAsync([FromBody] CreateVehiclesCommand command,
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
    public async Task<IResult> PutAsync([FromBody] UpdateVehiclesCommand command,
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

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(CommandResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IResult> DeleteAsync([FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteVehiclesCommand
        {
            Id = id,
        };

        var result = await _deleteCommandValidator
            .ValidateAsync(command, cancellationToken);

        if (!result.IsValid)
        {
            return result.Exception.TreatExceptionToResult(HttpContext);
        }

        await _publisher.PublishCommandAsync(command, cancellationToken);

        return Results.Accepted(GetLocationUri(command.SagaId), new CommandResponse(command.Id));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VehicleResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IResult> GetAsync([FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _vehicleProjectionFacade.GetAsync(x => x.Id == id, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!.TreatExceptionToResult(HttpContext);
        }

        return Results.Ok(entity.Value);
    }

    [HttpGet("{licensePlate}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VehicleProjection))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IResult> GetLicensePlateAsync([FromRoute] string licensePlate,
        CancellationToken cancellationToken = default)
    {
        var entity =
            await _vehicleProjectionFacade.GetAsync(x => x.LicensePlate == licensePlate, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!.TreatExceptionToResult(HttpContext);
        }

        return Results.Ok(entity.Value);
    }
}
