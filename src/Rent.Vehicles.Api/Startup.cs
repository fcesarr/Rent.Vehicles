using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

using MongoDB.Driver;

using RabbitMQ.Client;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Contexts;
using Rent.Vehicles.Entities.Contexts.Interfaces;
using Rent.Vehicles.Entities.Extensions;
using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Lib.Constants;
using Rent.Vehicles.Lib.Extensions;
using Rent.Vehicles.Lib.Serializers;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Messages.Types;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.DataServices;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Extensions;
using Rent.Vehicles.Services.Facades;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Validators;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Api;

[ExcludeFromCodeCoverage]
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration
    {
        get;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        //

        services.AddRouting(options => options.LowercaseUrls = true);

        services
            .AddCustomHealthCheck(Configuration)
            .AddDbContextDependencies<IDbContext,
                RentVehiclesContext>(Configuration.GetConnectionString("Sql") ?? string.Empty)
            // UserProjection
            .AddProjectionDomain<UserProjection,
                IUserProjectionDataService,
                UserProjectionDataService,
                IUserProjectionFacade,
                UserProjectionFacade>()
            .AddDataDomain<User, IUserValidator, UserValidator, IUserDataService, UserDataService>()
            // UserProjection
            // VehicleProjection
            .AddProjectionDomain<VehicleProjection,
                IVehicleProjectionDataService,
                VehicleProjectionDataService,
                IVehicleProjectionFacade,
                VehicleProjectionFacade>()
            .AddDataDomain<Vehicle, IVehicleValidator, VehicleValidator, IVehicleDataService, VehicleDataService>()
            // VehicleProjection
            // VehiclesForSpecificYearProjection
            .AddProjectionDomain<VehiclesForSpecificYearProjection,
                IVehiclesForSpecificYearProjectionDataService,
                VehiclesForSpecificYearProjectionDataService,
                IVehiclesForSpecificYearProjectionFacade,
                VehiclesForSpecificYearProjectionFacade>()
            // VehiclesForSpecificYearProjection
            // RentProjection
            .AddProjectionDomain<RentProjection,
                IRentProjectionDataService,
                RentProjectionDataService,
                IRentProjectionFacade,
                RentProjectionFacade>()
            .AddDataDomain<Entities.Rent, IRentValidator, RentValidator, IRentDataService, RentDataService>()
            // RentProjection
            // EventProjection
            .AddProjectionDomain<EventProjection,
                IEventProjectionDataService,
                EventProjectionDataService,
                IEventProjectionFacade,
                EventProjectionFacade>()
            .AddDataDomain<Event, IEventValidator, EventValidator, IEventDataService, EventDataService>()
            // EventProjection
            .AddAmqpLiteBroker(Configuration)
            .AddSingleton<IMongoDatabase>(service =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();

                var connectionString = configuration.GetConnectionString("NoSql") ?? string.Empty;

                MongoClient client = new(connectionString);

                var databaseName = MongoUrl.Create(connectionString).DatabaseName;

                return client.GetDatabase(databaseName);
            })
            .AddSingleton<IConnection>(service =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();

                var connectionString = configuration.GetConnectionString("Broker") ?? string.Empty;

                var factory = new ConnectionFactory
                {
                    Uri = new Uri(connectionString),
                    DispatchConsumersAsync = true,
                    ConsumerDispatchConcurrency = 100
                };

                return factory.CreateConnection();
            })
            .AddDefaultSerializer<MessagePackSerializer>()
            .AddScoped<IValidator<CreateRentCommand>, Validators.Validator<CreateRentCommand>>()
            .AddScoped<IValidator<CreateUserCommand>, Validators.Validator<CreateUserCommand>>()
            .AddScoped<IValidator<CreateVehiclesCommand>, Validators.Validator<CreateVehiclesCommand>>()
            .AddScoped<IValidator<DeleteVehiclesCommand>, Validators.Validator<DeleteVehiclesCommand>>()
            .AddScoped<IValidator<UpdateRentCommand>, Validators.Validator<UpdateRentCommand>>()
            .AddScoped<IValidator<UpdateUserCommand>, Validators.Validator<UpdateUserCommand>>()
            .AddScoped<IValidator<UpdateUserLicenseImageCommand>, Validators.Validator<UpdateUserLicenseImageCommand>>()
            .AddScoped<IValidator<UpdateVehiclesCommand>, Validators.Validator<UpdateVehiclesCommand>>();

        services.AddControllers();

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Example API", Version = "v1" });

            c.MapType<VehicleType>(() => new OpenApiSchema
            {
                Type = "string",
                Enum = Enum.GetNames(typeof(VehicleType))
                    .Select(name => new OpenApiString(name))
                    .Cast<IOpenApiAny>()
                    .ToList()
            });
            c.MapType<LicenseType>(() => new OpenApiSchema
            {
                Type = "string",
                Enum = Enum.GetNames(typeof(LicenseType))
                    .Select(name => new OpenApiString(name))
                    .Cast<IOpenApiAny>()
                    .ToList()
            });
        });

        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = problemDetaisContext =>
            {
                var webHostEnvironment =
                    problemDetaisContext.HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();

                if (problemDetaisContext.HttpContext.Response.StatusCode == StatusCodes.Status500InternalServerError
                    && !webHostEnvironment.IsDevelopment())
                {
                    problemDetaisContext.ProblemDetails.Detail = null;
                }
            };
        });

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        //
    }

    public void Configure(IApplicationBuilder app)
    {
        //
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks(HealthCheckUri.Ready,
                new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains(HealthCheckTag.Ready),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

            endpoints.MapHealthChecks(HealthCheckUri.Live,
                new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains(HealthCheckTag.Live),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

            endpoints.MapControllers();
        });

        app.UseDeveloperExceptionPage();

        app.UseSwagger();

        app.UseSwaggerUI();

        app.UseStaticFiles();

        app.UseHttpsRedirection();
        //
    }
}
