using System.Text.Json.Serialization;

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

using RabbitMQ.Client;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Contexts;
using Rent.Vehicles.Entities.Contexts.Interfaces;
using Rent.Vehicles.Entities.Extensions;
using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Lib.Serializers;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Messages.Types;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.DataServices;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Facades;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators;
using Rent.Vehicles.Services.Validators.Interfaces;
using Rent.Vehicles.Services.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Rent.Vehicles.Lib.Constants;
using Rent.Vehicles.Lib.Extensions;
using Serilog;
using System.Reflection;
using System.Diagnostics;
using Rent.Vehicles.Messages;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostBuilderContext, loggerConfiguration) =>
{
	var assembly = Assembly.GetExecutingAssembly();
	var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
	var version = fvi.FileVersion;

	loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration)
		.Enrich.WithProperty("Version", version);
});

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services
    .AddCustomHealthCheck(builder.Configuration)
    .AddDbContextDependencies<IDbContext,
        RentVehiclesContext>(builder.Configuration.GetConnectionString("Sql") ??string.Empty)
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
    .AddDataDomain<Rent.Vehicles.Entities.Rent, IRentValidator, RentValidator, IRentDataService, RentDataService>()
    // RentProjection
    // EventProjection
    .AddProjectionDomain<EventProjection,
        IEventProjectionDataService,
        EventProjectionDataService,
        IEventProjectionFacade,
        EventProjectionFacade>()
    .AddDataDomain<Rent.Vehicles.Entities.Event, IEventValidator, EventValidator, IEventDataService, EventDataService>()
    // EventProjection
    .AddAmqpLiteBroker(builder.Configuration)
    .AddSingleton<IMongoDatabase>(service =>
    {
        var configuration = service.GetRequiredService<IConfiguration>();

        var connectionString = configuration.GetConnectionString("NoSql") ?? string.Empty;

        MongoClient client = new(connectionString);

        var databaseName = MongoUrl.Create(connectionString).DatabaseName;

        return client.GetDatabase(databaseName);
    })
    .AddSingleton<IConnection>(service => {
        var configuration = service.GetRequiredService<IConfiguration>();

        var connectionString = configuration.GetConnectionString("Broker") ?? string.Empty;

        var factory =  new ConnectionFactory 
        {
            Uri = new Uri(connectionString),
            DispatchConsumersAsync = true,
            ConsumerDispatchConcurrency = 100
        };

        return factory.CreateConnection();
    })
    .AddDefaultSerializer<MessagePackSerializer>()
    .AddScoped<IValidator<CreateRentCommand>, Rent.Vehicles.Api.Validators.Validator<CreateRentCommand>>()
    .AddScoped<IValidator<CreateUserCommand>, Rent.Vehicles.Api.Validators.Validator<CreateUserCommand>>()
    .AddScoped<IValidator<CreateVehiclesCommand>, Rent.Vehicles.Api.Validators.Validator<CreateVehiclesCommand>>()
    .AddScoped<IValidator<DeleteVehiclesCommand>, Rent.Vehicles.Api.Validators.Validator<DeleteVehiclesCommand>>()
    .AddScoped<IValidator<UpdateRentCommand>, Rent.Vehicles.Api.Validators.Validator<UpdateRentCommand>>()
    .AddScoped<IValidator<UpdateUserCommand>, Rent.Vehicles.Api.Validators.Validator<UpdateUserCommand>>()
    .AddScoped<IValidator<UpdateUserLicenseImageCommand>, Rent.Vehicles.Api.Validators.Validator<UpdateUserLicenseImageCommand>>()
    .AddScoped<IValidator<UpdateVehiclesCommand>, Rent.Vehicles.Api.Validators.Validator<UpdateVehiclesCommand>>();
    
builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
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

builder.Services.AddProblemDetails(options =>
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

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

BsonClassMap.RegisterClassMap<Event>(map =>
{
    map.AutoMap();
    map.MapProperty(x => x.SagaId).SetSerializer(new GuidSerializer(BsonType.String));
});


var app = builder.Build();

app.UseRouting();

app.MapHealthChecks(HealthCheckUri.Ready, new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains(HealthCheckTag.Ready),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks(HealthCheckUri.Live, new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains(HealthCheckTag.Live),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseDeveloperExceptionPage();

app.UseSwagger();

app.UseSwaggerUI();

app.UseStaticFiles();

app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();

public partial class Program
{
    
}


// using System.Text.Json.Serialization;

// using Microsoft.OpenApi.Any;
// using Microsoft.OpenApi.Models;

// using MongoDB.Bson;
// using MongoDB.Bson.Serialization;
// using MongoDB.Bson.Serialization.Serializers;
// using MongoDB.Driver;

// using RabbitMQ.Client;

// using Rent.Vehicles.Entities;
// using Rent.Vehicles.Entities.Contexts;
// using Rent.Vehicles.Entities.Contexts.Interfaces;
// using Rent.Vehicles.Entities.Extensions;
// using Rent.Vehicles.Entities.Projections;
// using Rent.Vehicles.Lib.Serializers;
// using Rent.Vehicles.Lib.Serializers.Interfaces;
// using Rent.Vehicles.Messages.Commands;
// using Rent.Vehicles.Messages.Types;
// using Rent.Vehicles.Services;
// using Rent.Vehicles.Services.DataServices;
// using Rent.Vehicles.Services.DataServices.Interfaces;
// using Rent.Vehicles.Services.Facades;
// using Rent.Vehicles.Services.Facades.Interfaces;
// using Rent.Vehicles.Services.Interfaces;
// using Rent.Vehicles.Services.Repositories;
// using Rent.Vehicles.Services.Repositories.Interfaces;
// using Rent.Vehicles.Services.Validators;
// using Rent.Vehicles.Services.Validators.Interfaces;
// using Rent.Vehicles.Services.Extensions;
// using Microsoft.AspNetCore.Diagnostics.HealthChecks;
// using HealthChecks.UI.Client;
// using Rent.Vehicles.Lib.Constants;
// using Rent.Vehicles.Lib.Extensions;
// using Serilog;
// using System.Reflection;
// using System.Diagnostics;
// using Rent.Vehicles.Messages;
// using Rent.Vehicles.Api;

// BsonClassMap.RegisterClassMap<Event>(map =>
// {
//     map.AutoMap();
//     map.MapProperty(x => x.SagaId).SetSerializer(new GuidSerializer(BsonType.String));
// });


// await Host.CreateDefaultBuilder(args)
//     .ConfigureWebHostDefaults(webBuilder =>
//         webBuilder.UseStartup<Startup>())
//     .UseSerilog((hostBuilderContext, loggerConfiguration) =>
//     {
//         var assembly = Assembly.GetExecutingAssembly();
//         var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
//         var version = fvi.FileVersion;

//         loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration)
//             .Enrich.WithProperty("Version", version);
//     })
//     .UseConsoleLifetime()
//     .Build()
//     .RunAsync();

