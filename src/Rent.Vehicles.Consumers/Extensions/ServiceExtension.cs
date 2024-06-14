
using Rent.Vehicles.Entities;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Consumers.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddCreateService<TEntity>(this IServiceCollection services) where TEntity : Entity
        => services.AddSingleton<IRepository<TEntity>, EntityFrameworkRepository<TEntity>>()
            .AddSingleton<IValidator<TEntity>, Validator<TEntity>>()
            .AddSingleton<ICreateService<TEntity>, SqlService<TEntity>>();
}