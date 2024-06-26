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
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Producers.RabbitMQ;
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

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContextDependencies<IDbContext,
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
    .AddSingleton<IPublisher>(service => 
    {
        var connection = service.GetRequiredService<IConnection>();
        var serializer = service.GetRequiredService<ISerializer>();
        return new RabbitMQPublisher(connection.CreateModel(), serializer);
    })
    .AddSingleton<IMongoDatabase>(service =>
    {
        var configuration = service.GetRequiredService<IConfiguration>();

        var connectionString = configuration.GetConnectionString("NoSql") ?? string.Empty;

        MongoClient client = new(connectionString);

        var databaseName = MongoUrl.Create(connectionString).DatabaseName;

        return client.GetDatabase(databaseName);
    })
    .AddSingleton<IConnection>(service => {
        var factory =  new ConnectionFactory 
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "admin",
            Password = "nimda",
            VirtualHost = "/",
            RequestedConnectionTimeout = TimeSpan.FromSeconds(30),
            SocketReadTimeout = TimeSpan.FromSeconds(30),
            SocketWriteTimeout = TimeSpan.FromSeconds(30)
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

builder.Services.AddRouting(options => options.LowercaseUrls = true);

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseRouting();

app.MapControllers();

app.Run();
