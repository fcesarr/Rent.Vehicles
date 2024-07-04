using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Projections.Events;
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
        using var serviceScope = _serviceScopeFactory.CreateScope();

        var serviceProvider = serviceScope.ServiceProvider;

        var service = serviceProvider.GetRequiredService<IUserProjectionFacade>();

        var entity = await service.UpdateAsync(@event, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return Task.CompletedTask;
    }
}
