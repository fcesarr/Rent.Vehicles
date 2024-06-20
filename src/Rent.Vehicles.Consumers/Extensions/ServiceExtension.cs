
using Rent.Vehicles.Entities;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Consumers.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddDataDomain<TEntity>(this IServiceCollection services)
        where TEntity : Entity
        => services.AddSingleton<IValidator<TEntity>, Validator<TEntity>>()
            .AddSingleton<IRepository<TEntity>, EntityFrameworkRepository<TEntity>>()
            .AddSingleton<IService<TEntity>, Service<TEntity>>();

    public static IServiceCollection AddDataDomain<TEntity, TIValidator, TValidatorImplementation, TIService, TServiceImplementation>(this IServiceCollection services) 
        where TEntity : Entity
        where TIValidator : class, IValidator<TEntity>
        where TValidatorImplementation : Validator<TEntity>, TIValidator
        where TIService : class, IService<TEntity>
        where TServiceImplementation : Service<TEntity>, TIService
        => services.AddSingleton<TIValidator, TValidatorImplementation>()
            .AddSingleton<IRepository<TEntity>, EntityFrameworkRepository<TEntity>>()
            .AddSingleton<TIService, TServiceImplementation>();

    public static IServiceCollection AddProjectionDomain<TEntity>(this IServiceCollection services)
        where TEntity : Entity
        => services.AddSingleton<IValidator<TEntity>, Validator<TEntity>>()
            .AddSingleton<IRepository<TEntity>, MongoRepository<TEntity>>()
            .AddSingleton<IService<TEntity>, Service<TEntity>>();

    public static IServiceCollection AddProjectionDomain<TEntity, TIService, TServiceImplementation>(this IServiceCollection services) 
        where TEntity : Entity
        where TIService : class, IService<TEntity>
        where TServiceImplementation : Service<TEntity>, TIService
        => services.AddSingleton<IValidator<TEntity>, Validator<TEntity>>()
            .AddSingleton<IRepository<TEntity>, MongoRepository<TEntity>>()
            .AddSingleton<TIService, TServiceImplementation>();

    public static IServiceCollection AddDefaultSerializer<TImplementation>(this IServiceCollection services)
        where TImplementation : class, ISerializer
        => services.AddSingleton<ISerializer, TImplementation>();

}