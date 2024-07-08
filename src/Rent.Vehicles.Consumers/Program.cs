using MongoDB.Driver;

using RabbitMQ.Client;

using Rent.Vehicles.Consumers;
using Rent.Vehicles.Consumers.Commands.BackgroundServices;
using Rent.Vehicles.Consumers.Events.BackgroundServices;
using Rent.Vehicles.Services.Extensions;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Contexts;
using Rent.Vehicles.Entities.Contexts.Interfaces;
using Rent.Vehicles.Entities.Extensions;
using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Lib.Serializers;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.DataServices;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Facades;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Settings;
using Rent.Vehicles.Services.Validators;
using Rent.Vehicles.Services.Validators.Interfaces;
using Rent.Vehicles.Lib.Constants;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Rent.Vehicles.Lib.Extensions;
using Serilog;
using System.Reflection;
using System.Diagnostics;
using System.CommandLine;
using Rent.Vehicles.Consumers.Types;
using Rent.Vehicles.Consumers.Settings;

var rootCommand = new RootCommand("Sample command-line app");

var consumerTypeOption = new Option<ConsumerType>(
	new string[] {"-t", "--type"},
	description: "Type of consumer",
	getDefaultValue: () => ConsumerType.Both);

rootCommand.AddOption(consumerTypeOption);

var bufferSizeOption = new Option<int>(
	new string[] {"-s", "--size"},
	description: "Buffer Size",
	getDefaultValue: () => 5);

rootCommand.AddOption(bufferSizeOption);

var toExcludedOption = new Option<IEnumerable<string>>(
	new string[] {"-e", "--excluded"},
	description: "To Excluded",
	getDefaultValue: () => []);

rootCommand.AddOption(toExcludedOption);

var toIncludedOption = new Option<IEnumerable<string>>(
	new string[] {"-i", "--included"},
	description: "To Included",
	getDefaultValue: () => []);

rootCommand.AddOption(toIncludedOption);

rootCommand.SetHandler(async (consumerType, bufferSize, toExcluded, toIncluded) =>
{
    var builder = WebApplication.CreateBuilder();

    builder.Host.UseSerilog((hostBuilderContext, loggerConfiguration) =>
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
        var version = fvi.FileVersion;

        loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration)
            .Enrich.WithProperty("Version", version);
    });

    builder.Services.Configure<ConsumerSetting>(options =>
    {
        options.Type = consumerType;
        options.BufferSize = bufferSize;
        options.ToExcluded = toExcluded;
        options.ToIncluded = toIncluded;
    });

    builder.Services.AddRouting(options => options.LowercaseUrls = true);

    builder.Services
        .AddCustomHealthCheck(builder.Configuration)
        .AddDbContextDependencies<IDbContext, RentVehiclesContext>(builder.Configuration.GetConnectionString("Sql") ??
                                                                string.Empty)
        .AddTransient<IPeriodicTimer>(service =>
        {
            PeriodicTimer periodicTimer = new(TimeSpan.FromMilliseconds(500));

            return new Rent.Vehicles.Consumers.Utils.PeriodicTimer(periodicTimer);
        })
        // UserProjection
        .AddProjectionDomain<UserProjection,
            IUserProjectionDataService,
            UserProjectionDataService,
            IUserProjectionFacade,
            UserProjectionFacade>()
        // UserProjection
        // VehicleProjection
        .AddProjectionDomain<VehicleProjection,
            IVehicleProjectionDataService,
            VehicleProjectionDataService,
            IVehicleProjectionFacade,
            VehicleProjectionFacade>()
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
        // RentProjection
        // EventProjection
        .AddProjectionDomain<EventProjection,
            IEventProjectionDataService,
            EventProjectionDataService,
            IEventProjectionFacade,
            EventProjectionFacade>()
        // EventProjection
        // Event
        .AddDataDomain<Event,
            IEventValidator,
            EventValidator,
            IEventDataService,
            EventDataService,
            IEventFacade,
            EventFacade>()
        // Event
        // Command
        .AddDataDomain<Rent.Vehicles.Entities.Command,
            ICommandValidator,
            CommandValidator,
            ICommandDataService,
            CommandDataService,
            ICommandFacade,
            CommandFacade>()
        // Command
        // Vehicle
        .AddDataDomain<Vehicle,
            IVehicleValidator,
            VehicleValidator,
            IVehicleDataService,
            VehicleDataService,
            IVehicleFacade,
            VehicleFacade>()
        // Vehicle
        // User
        .AddDataDomain<User,
            IUserValidator,
            UserValidator,
            IUserDataService,
            UserDataService,
            IUserFacade,
            UserFacade>()
        // User
        // Rent
        .AddDataDomain<Rent.Vehicles.Entities.Rent,
            IRentValidator,
            RentValidator,
            IRentDataService,
            RentDataService,
            IRentFacade,
            RentFacade>()
        // Rent
        // RentPlane
        .AddDataDomain<RentalPlane, IRentalPlaneValidator, RentalPlaneValidator, IRentalPlaneDataService, RentalPlaneDataService>()
        // RentPlane
        .AddSingleton<IUploadService, FileUploadService>()
        .AddSingleton<Func<string, byte[], CancellationToken, Task>>(service => File.WriteAllBytesAsync)
        .AddSingleton<IMongoDatabase>(service =>
        {
            var configuration = service.GetRequiredService<IConfiguration>();

            var connectionString = configuration.GetConnectionString("NoSql") ?? string.Empty;

            MongoClient client = new(connectionString);

            var databaseName = MongoUrl.Create(connectionString).DatabaseName;

            return client.GetDatabase(databaseName);
        })
        .AddAmqpLiteBroker(builder.Configuration)
        .AddDefaultSerializer<MessagePackSerializer>()
        .AddHostedService<CreateRentCommandBackgroundService>()
        .AddHostedService<CreateUserCommandBackgroundService>()
        .AddHostedService<CreateVehiclesCommandBackgroundService>()
        .AddHostedService<DeleteVehiclesCommandBackgroundService>()
        .AddHostedService<UpdateRentCommandBackgroundService>()
        .AddHostedService<UpdateUserCommandBackgroundService>()
        .AddHostedService<UpdateUserLicenseImageCommandBackgroundService>()
        .AddHostedService<UpdateVehiclesCommandBackgroundService>()
        .AddHostedService<CreateRentEventBackgroundService>()
        .AddHostedService<CreateRentProjectionEventBackgroundService>()
        .AddHostedService<CreateUserEventBackgroundService>()
        .AddHostedService<CreateUserProjectionEventBackgroundService>()
        .AddHostedService<CreateVehiclesEventBackgroundService>()
        .AddHostedService<CreateVehiclesForSpecificYearEventBackgroundService>()
        .AddHostedService<CreateVehiclesForSpecificYearProjectionEventBackgroundService>()
        .AddHostedService<CreateVehiclesProjectionEventBackgroundService>()
        .AddHostedService<DeleteVehiclesEventBackgroundService>()
        .AddHostedService<DeleteVehiclesProjectionEventBackgroundService>()
        .AddHostedService<EventBackgroundService>()
        .AddHostedService<EventProjectionEventBackgroundService>()
        .AddHostedService<UpdateRentEventBackgroundService>()
        .AddHostedService<UpdateRentProjectionEventBackgroundService>()
        .AddHostedService<UpdateUserEventBackgroundService>()
        .AddHostedService<UpdateUserLicenseImageEventBackgroundService>()
        .AddHostedService<UpdateUserProjectionEventBackgroundService>()
        .AddHostedService<UpdateVehiclesEventBackgroundService>()
        .AddHostedService<UpdateVehiclesProjectionEventBackgroundService>()
        .AddHostedService<UploadUserLicenseImageEventBackgroundService>();

    builder.Services.AddOptions<FileUploadSetting>()
        .BindConfiguration(nameof(FileUploadSetting))
        .ValidateDataAnnotations()
        .ValidateOnStart();

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

    await app.RunAsync();
}, consumerTypeOption, bufferSizeOption, toExcludedOption, toIncludedOption);

return await rootCommand.InvokeAsync(args);
