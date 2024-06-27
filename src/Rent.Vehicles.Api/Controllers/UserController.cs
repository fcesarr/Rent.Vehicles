
using Microsoft.AspNetCore.Mvc;

using Rent.Vehicles.Api.Extensions;
using Rent.Vehicles.Api.Responses;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Responses;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Api.Controllers;

[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly IPublisher _publisher;

    private readonly IValidator<CreateUserCommand> _createCommandValidator;

    private readonly IValidator<UpdateUserCommand> _updateCommandValidator;

    private readonly IUserFacade _facade;

    public UserController(IPublisher publisher,
        IValidator<CreateUserCommand> createCommandValidator,
        IValidator<UpdateUserCommand> updateCommandValidator,
        IUserFacade facade)
    {
        _publisher = publisher;
        _createCommandValidator = createCommandValidator;
        _updateCommandValidator = updateCommandValidator;
        _facade = facade;
    }

    [HttpPost]
	[ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(CommandResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IResult> PostAsync([FromBody]CreateUserCommand command,
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
	public async Task<IResult> PutAsync([FromBody]UpdateUserCommand command,
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

    [HttpGet("{id:guid}")]
	[ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(UserResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IResult> GetAsync([FromRoute]Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _facade.GetAsync(x => x.Id == id, cancellationToken);

        if(!entity.IsSuccess)
            return entity.Exception!.TreatExceptionToResult(HttpContext);

        return Results.Ok(entity.Value);
    }
}