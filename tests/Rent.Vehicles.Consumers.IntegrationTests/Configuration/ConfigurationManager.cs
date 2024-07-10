using Microsoft.Extensions.Configuration;

namespace Rent.Vehicles.Consumers.IntegrationTests.Configuration;

public class ConfigurationManager
{
    private static ConfigurationManager? _configurationManager;
    private readonly IConfiguration _configuration;

    private ConfigurationManager(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public static ConfigurationManager GetInstance()
    {
        if (_configurationManager != null)
        {
            return _configurationManager;
        }

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"), false)
            .Build();

        _configurationManager = new ConfigurationManager(configuration);

        return _configurationManager;
    }

    public IConfiguration GetConfiguration()
    {
        return _configuration;
    }
}
