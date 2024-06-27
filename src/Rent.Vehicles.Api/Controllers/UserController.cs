using Microsoft.AspNetCore.Mvc;

using Rent.Vehicles.Api.Extensions;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Responses;
using Rent.Vehicles.Services.Validators.Interfaces;

using CommandResponse = Rent.Vehicles.Api.Responses.CommandResponse;

namespace Rent.Vehicles.Api.Controllers;

[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly IValidator<CreateUserCommand> _createCommandValidator;
    private readonly IUserFacade _facade;
    private readonly IUserProjectionFacade _projectionFacade;
    private readonly IPublisher _publisher;
    private readonly IValidator<UpdateUserCommand> _updateCommandValidator;

    private readonly IValidator<UpdateUserLicenseImageCommand> _updateImageCommandValidator;

    public UserController(IValidator<CreateUserCommand> createCommandValidator, IUserFacade facade,
        IUserProjectionFacade projectionFacade, IPublisher publisher,
        IValidator<UpdateUserCommand> updateCommandValidator,
        IValidator<UpdateUserLicenseImageCommand> updateImageCommandValidator)
    {
        _createCommandValidator = createCommandValidator;
        _facade = facade;
        _projectionFacade = projectionFacade;
        _publisher = publisher;
        _updateCommandValidator = updateCommandValidator;
        _updateImageCommandValidator = updateImageCommandValidator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(CommandResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IResult> PostAsync([FromBody] CreateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        command.SagaId = Guid.NewGuid();

        var result = await _createCommandValidator
            .ValidateAsync(command, cancellationToken);

        if (!result.IsValid)
        {
            return result.Exception.TreatExceptionToResult(HttpContext);
        }

        await _publisher.PublishCommandAsync(command, cancellationToken);

        var locationUri = $"/Events/status/{command.SagaId}";

        return Results.Accepted(locationUri, new CommandResponse(command.Id));
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(CommandResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IResult> PutAsync([FromBody] UpdateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        command.SagaId = Guid.NewGuid();

        var result = await _updateCommandValidator
            .ValidateAsync(command, cancellationToken);

        if (!result.IsValid)
        {
            return result.Exception.TreatExceptionToResult(HttpContext);
        }

        await _publisher.PublishCommandAsync(command, cancellationToken);

        var locationUri = $"/Events/status/{command.SagaId}";

        return Results.Accepted(locationUri, new CommandResponse(command.Id));
    }

    [HttpPut("upload")]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(CommandResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IResult> PutUploadAsync([FromBody] UpdateUserLicenseImageCommand command,
        CancellationToken cancellationToken = default)
    {
        command.SagaId = Guid.NewGuid();

        var result = await _updateImageCommandValidator
            .ValidateAsync(command, cancellationToken);

        if (!result.IsValid)
        {
            return result.Exception.TreatExceptionToResult(HttpContext);
        }

        await _publisher.PublishCommandAsync(command, cancellationToken);

        var locationUri = $"/Events/status/{command.SagaId}";

        return Results.Accepted(locationUri, new CommandResponse(command.Id));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(UserResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IResult> GetAsync([FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _projectionFacade.GetAsync(x => x.Id == id, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!.TreatExceptionToResult(HttpContext);
        }

        return Results.Ok(entity.Value);
    }
}
