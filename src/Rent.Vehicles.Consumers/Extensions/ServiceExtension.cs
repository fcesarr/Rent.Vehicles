
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
    public static IServiceCollection AddSqlService<TEntity>(this IServiceCollection services) where TEntity : Entity
        => services.AddSqlService<TEntity, Validator<TEntity>>();

    public static IServiceCollection AddSqlService<TEntity, TValidator>(this IServiceCollection services) 
        where TEntity : Entity
        where TValidator : Validator<TEntity>
        => services.AddSingleton<IRepository<TEntity>, EntityFrameworkRepository<TEntity>>()
            .AddSingleton<IValidator<TEntity>, TValidator>()
            .AddSingleton<ICreateService<TEntity>, Service<TEntity>>()
            .AddSingleton<IDeleteService<TEntity>, Service<TEntity>>()
            .AddSingleton<IUpdateService<TEntity>, Service<TEntity>>();
    public static IServiceCollection AddNoSqlService<TEntity>(this IServiceCollection services) 
        where TEntity : Entity
        => services.AddSingleton<IRepository<TEntity>, MongoRepository<TEntity>>()
            .AddSingleton<IValidator<TEntity>, Validator<TEntity>>()
            .AddSingleton<ICreateService<TEntity>, Service<TEntity>>()
            .AddSingleton<IDeleteService<TEntity>, Service<TEntity>>()
            .AddSingleton<IUpdateService<TEntity>, Service<TEntity>>();
}