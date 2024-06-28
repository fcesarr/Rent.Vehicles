
using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.Facades.Interfaces;

namespace Rent.Vehicles.Consumers.Events.BackgroundServices;

public class UpdateUserProjectionEventBackgroundService : HandlerEventServicePublishBackgroundService<
    UpdateUserProjectionEvent>
{
    public UpdateUserProjectionEventBackgroundService(ILogger<HandlerEventServicePublishBackgroundService<UpdateUserProjectionEvent>> logger, IConsumer channel, IPeriodicTimer periodicTimer, ISerializer serializer, IPublisher publisher, IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher, serviceScopeFactory)
    {
    }

    protected async override Task<Result<Task>> HandlerMessageAsync(UpdateUserProjectionEvent @event, CancellationToken cancellationToken = default)
    {
        var _service = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<IUserProjectionFacade>();

        var entity = await _service.UpdateAsync(@event, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return Task.CompletedTask;
    }
}
