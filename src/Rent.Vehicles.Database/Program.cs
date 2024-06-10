using DbUp;
using System.Reflection;

var connectionString =
    args.FirstOrDefault()
    ?? "Server=localhost;Port=5432;User Id=postgres;Password=postgres;Database=rent-vehicles;Timeout=1024;CommandTimeout=10000;Pooling=true;Minimum Pool Size=10;Maximum Pool Size=20;Application Name=Rent.Vehicles.Database";

EnsureDatabase.For.PostgresqlDatabase(connectionString);

var upgrader =
    DeployChanges.To
        .PostgresqlDatabase(connectionString)
        .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
        .LogToConsole()
        .Build();

var result = upgrader.PerformUpgrade();

if (!result.Successful)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(result.Error);
    Console.ResetColor();      
    return -1;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Success!");
Console.ResetColor();
return 0;