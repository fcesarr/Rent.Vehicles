using Microsoft.Extensions.DependencyInjection;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddDataDomain<TEntity>(this IServiceCollection services)
        where TEntity : Entity
    {
        return services.AddScoped<IValidator<TEntity>, Validator<TEntity>>()
            .AddScoped<IRepository<TEntity>, EntityFrameworkRepository<TEntity>>()
            .AddScoped<IDataService<TEntity>, DataService<TEntity>>();
    }

    public static IServiceCollection AddDataDomain<TEntity, TIValidator, TValidatorImplementation, TIService,
        TServiceImplementation>(this IServiceCollection services)
        where TEntity : Entity
        where TIValidator : class, IValidator<TEntity>
        where TValidatorImplementation : Validator<TEntity>, TIValidator
        where TIService : class, IDataService<TEntity>
        where TServiceImplementation : DataService<TEntity>, TIService
    {
        return services.AddScoped<TIValidator, TValidatorImplementation>()
            .AddScoped<IRepository<TEntity>, EntityFrameworkRepository<TEntity>>()
            .AddScoped<TIService, TServiceImplementation>();
    }

    public static IServiceCollection AddDataDomain<TEntity, TIValidator, TValidatorImplementation, TIService,
        TServiceImplementation, TIFacade, TFacadeImplementation>(this IServiceCollection services)
        where TEntity : Entity
        where TIValidator : class, IValidator<TEntity>
        where TValidatorImplementation : Validator<TEntity>, TIValidator
        where TIService : class, IDataService<TEntity>
        where TServiceImplementation : DataService<TEntity>, TIService
        where TIFacade : class, IFacade
        where TFacadeImplementation : class, TIFacade
    {
        return services.AddDataDomain<TEntity,
                TIValidator,
                TValidatorImplementation,
                TIService,
                TServiceImplementation>()
            .AddScoped<TIFacade, TFacadeImplementation>();
    }

    public static IServiceCollection AddProjectionDomain<TEntity>(this IServiceCollection services)
        where TEntity : Entity
    {
        return services.AddScoped<IValidator<TEntity>, Validator<TEntity>>()
            .AddScoped<IRepository<TEntity>, MongoRepository<TEntity>>()
            .AddScoped<IDataService<TEntity>, DataService<TEntity>>();
    }

    public static IServiceCollection AddProjectionDomain<TEntity, TIService,
        TServiceImplementation, TIFacade, TFacadeImplementation>(this IServiceCollection services)
        where TEntity : Entity
        where TIService : class, IDataService<TEntity>
        where TServiceImplementation : DataService<TEntity>, TIService
        where TIFacade : class, IFacade
        where TFacadeImplementation : class, TIFacade
    {
        return services.AddScoped<IValidator<TEntity>, Validator<TEntity>>()
            .AddScoped<IRepository<TEntity>, MongoRepository<TEntity>>()
            .AddScoped<TIService, TServiceImplementation>()
            .AddScoped<TIFacade, TFacadeImplementation>();
    }

    public static IServiceCollection AddDefaultSerializer<TImplementation>(this IServiceCollection services)
        where TImplementation : class, ISerializer
    {
        return services.AddSingleton<ISerializer, TImplementation>();
    }
}
