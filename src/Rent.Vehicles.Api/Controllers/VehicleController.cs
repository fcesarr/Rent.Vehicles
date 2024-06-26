using Microsoft.AspNetCore.Mvc;

using Rent.Vehicles.Api.Extensions;
using Rent.Vehicles.Api.Responses;
using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Api.Controllers;

[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/[controller]")]
public class VehicleController : Controller
{
    private readonly IPublisher _publisher;

    private readonly IValidator<CreateVehiclesCommand> _createCommandValidator;

    private readonly IValidator<UpdateVehiclesCommand> _updateCommandValidator;

    private readonly IValidator<DeleteVehiclesCommand> _deleteCommandValidator;

    private readonly IDataService<VehicleProjection> _dataService;

    public VehicleController(IPublisher publisher, IValidator<CreateVehiclesCommand> createCommandValidator, IValidator<UpdateVehiclesCommand> updateCommandValidator, IValidator<DeleteVehiclesCommand> deleteCommandValidator, IDataService<VehicleProjection> dataService)
    {
        _publisher = publisher;
        _createCommandValidator = createCommandValidator;
        _updateCommandValidator = updateCommandValidator;
        _deleteCommandValidator = deleteCommandValidator;
        _dataService = dataService;
    }

    [HttpPost]
	[ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(CommandResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IResult> PostAsync([FromBody]CreateVehiclesCommand command,
        CancellationToken cancellationToken = default)
    {
        command.SagaId = Guid.NewGuid();

        var result = await _createCommandValidator
            .ValidateAsync(command, cancellationToken);

        if(!result.IsValid)
            return result.Exception.TreatExceptionToResult(HttpContext);

        await _publisher.PublishCommandAsync(command, cancellationToken);

        string locationUri = $"/Events/status/{command.SagaId}";

        return Results.Accepted(locationUri, new CommandResponse(command.Id));
    }

    [HttpPut]
	[ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(CommandResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IResult> PutAsync([FromBody]UpdateVehiclesCommand command,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        command.SagaId = Guid.NewGuid();

        var result = await _updateCommandValidator
            .ValidateAsync(command, cancellationToken);

        if(!result.IsValid)
            return result.Exception.TreatExceptionToResult(HttpContext);


        await _publisher.PublishCommandAsync(command, cancellationToken);

        string locationUri = $"/Events/status/{command.SagaId}";

        return Results.Accepted(locationUri, new CommandResponse(command.Id));
    }

    [HttpDelete]
	[ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(CommandResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IResult> DeleteAsync([FromBody]DeleteVehiclesCommand command,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        command.SagaId = Guid.NewGuid();

        var result = await _deleteCommandValidator
            .ValidateAsync(command, cancellationToken);

        if(!result.IsValid)
            return result.Exception.TreatExceptionToResult(HttpContext);


        await _publisher.PublishCommandAsync(command, cancellationToken);

        string locationUri = $"/Events/status/{command.SagaId}";

        return Results.Accepted(locationUri, new CommandResponse(command.Id));
    }

    [HttpGet("{id:guid}")]
	[ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(VehicleProjection))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IResult> GetAsync([FromRoute]Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dataService.GetAsync(x => x.Id == id, cancellationToken);

        if(!entity.IsSuccess)
        {
            return entity.Exception!.TreatExceptionToResult(HttpContext);
        }

        return Results.Ok(entity.Value);
    }

    [HttpGet("{licensePlate:string}")]
	[ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(VehicleProjection))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IResult> GetLicensePlateAsync([FromRoute]string licensePlate,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dataService.GetAsync(x => x.LicensePlate == licensePlate, cancellationToken);

        if(!entity.IsSuccess)
        {
            return entity.Exception!.TreatExceptionToResult(HttpContext);
        }

        return Results.Ok(entity.Value);
    }
}