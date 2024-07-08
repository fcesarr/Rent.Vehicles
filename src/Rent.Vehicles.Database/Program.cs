

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Rent.Vehicles.Entities.Extensions;
using Rent.Vehicles.Entities.Contexts;
using Rent.Vehicles.Entities.Contexts.Interfaces;
using Rent.Vehicles.Entities.Factories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", true)
	.AddJsonFile("appsettings.Docker.json", true)
	.AddJsonFile("appsettings.Development.json", true);

var sqlConnectionString = builder.Configuration
    .GetConnectionString("Sql") ?? throw new Exception("Connection String Empty");

builder.Services
    .AddDbContextDependencies<IDbContext, RentVehiclesContext>(sqlConnectionString);

var host = builder
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation(sqlConnectionString);

var factory = host.Services.GetRequiredService<IDbContextFactory>();

var context = await factory.CreateDbContextAsync();

await context.Database.MigrateAsync();

context.Dispose();

