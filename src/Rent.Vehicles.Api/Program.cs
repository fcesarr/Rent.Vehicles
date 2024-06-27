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

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IPublisher, Publisher>()
    .AddSingleton<ISerializer, MessagePackSerializer>()
    .AddSingleton<IModel>(service =>
    {
        ConnectionFactory factory = new()
        {
            HostName = "localhost", Port = 5672, UserName = "admin", Password = "nimda"
        };
        IConnection? connection = factory.CreateConnection();
        return connection.CreateModel();
    })
    .AddDbContextDependencies<IDbContext, RentVehiclesContext>(builder.Configuration.GetConnectionString("Sql") ??
                                                               string.Empty)
    .AddSingleton<IMongoDatabase>(service =>
    {
        IConfiguration configuration = service.GetRequiredService<IConfiguration>();

        string connectionString = configuration.GetConnectionString("NoSql") ?? string.Empty;

        MongoClient client = new(connectionString);

        return client.GetDatabase("rent");
    })
    .AddScoped<IValidator<Event>, EventValidator>()
    .AddScoped<IRepository<Event>, EntityFrameworkRepository<Event>>()
    .AddScoped<IDataService<Event>, DataService<Event>>()
    .AddScoped<IEventFacade, EventFacade>()
    .AddScoped<IUserValidator, UserValidator>()
    .AddScoped<ILicenseImageService, LicenseImageService>()
    .AddScoped<IUploadService, FileUploadService>()
    .AddScoped<Func<string, byte[], CancellationToken, Task>>(service => File.WriteAllBytesAsync)
    .AddScoped<IRepository<User>, EntityFrameworkRepository<User>>()
    .AddScoped<IUserDataService, UserDataService>()
    .AddScoped<IUserFacade, UserFacade>()
    .AddScoped<IRentValidator, RentValidator>()
    .AddScoped<IRepository<Rent.Vehicles.Entities.Rent>, EntityFrameworkRepository<Rent.Vehicles.Entities.Rent>>()
    .AddScoped<IRentDataService, RentDataService>()
    .AddScoped<IValidator<RentalPlane>, Validator<RentalPlane>>()
    .AddScoped<IRepository<RentalPlane>, EntityFrameworkRepository<RentalPlane>>()
    .AddScoped<IDataService<RentalPlane>, DataService<RentalPlane>>()
    .AddScoped<IVehicleDataService, VehicleDataService>()
    .AddScoped<IRentFacade, RentFacade>()
    .AddScoped<IVehicleValidator, VehicleValidator>()
    .AddScoped<IRepository<Vehicle>, EntityFrameworkRepository<Vehicle>>()
    .AddScoped<IRepository<VehicleProjection>, MongoRepository<VehicleProjection>>()
    .AddScoped<IDataService<VehicleProjection>, DataService<VehicleProjection>>()
    .AddScoped<IValidator<VehicleProjection>, Rent.Vehicles.Services.Validators.Validator<VehicleProjection>>()
    .AddScoped<IValidator<CreateUserCommand>, Rent.Vehicles.Api.Validators.Validator<CreateUserCommand>>()
    .AddScoped<IValidator<UpdateUserCommand>, Rent.Vehicles.Api.Validators.Validator<UpdateUserCommand>>()
    .AddScoped<IValidator<CreateRentCommand>, Rent.Vehicles.Api.Validators.Validator<CreateRentCommand>>()
    .AddScoped<IValidator<UpdateRentCommand>, Rent.Vehicles.Api.Validators.Validator<UpdateRentCommand>>()
    .AddScoped<IValidator<CreateVehiclesCommand>, Rent.Vehicles.Api.Validators.Validator<CreateVehiclesCommand>>()
    .AddScoped<IValidator<UpdateVehiclesCommand>, Rent.Vehicles.Api.Validators.Validator<UpdateVehiclesCommand>>()
    .AddScoped<IValidator<DeleteVehiclesCommand>, Rent.Vehicles.Api.Validators.Validator<DeleteVehiclesCommand>>();

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
        IWebHostEnvironment webHostEnvironment =
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


WebApplication app = builder.Build();

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