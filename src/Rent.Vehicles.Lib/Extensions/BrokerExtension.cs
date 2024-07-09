using System.Diagnostics.CodeAnalysis;

using Amqp;
using Amqp.Framing;
using Amqp.Types;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Lib.Responses;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Lib;
using Microsoft.Extensions.Logging;

namespace Rent.Vehicles.Lib.Extensions;

[ExcludeFromCodeCoverage]
public static class BrokerExtension
{
    public static IServiceCollection AddARabbitMqBroker(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddSingleton<RabbitMQ.Client.IConnection>(service => {
                var configuration = service.GetRequiredService<IConfiguration>();

                var connectionString = configuration.GetConnectionString("Broker") ?? string.Empty;

                var factory =  new RabbitMQ.Client.ConnectionFactory 
                {
                    Uri = new Uri(connectionString),
                    DispatchConsumersAsync = true,
                    ConsumerDispatchConcurrency = 100,
                    UseBackgroundThreadsForIO = false
                };

                return factory.CreateConnection();
            })
            .AddTransient<IConsumer>(service =>
            {
                var connection = service.GetRequiredService<RabbitMQ.Client.IConnection>();
                return new RabbitMQConsumer(connection.CreateModel());
            })
            .AddSingleton<IPublisher>(service => 
            {
                var connection = service.GetRequiredService<RabbitMQ.Client.IConnection>();
                var serializer = service.GetRequiredService<ISerializer>();
                return new RabbitMQPublisher(connection.CreateModel(), serializer);
            });
    }

    public static IServiceCollection AddAmqpLiteBroker(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddSingleton<ISession>(services => 
            {
                var brokerConnectionString = configuration.GetConnectionString("Broker") ?? string.Empty;

                var address = new Address(brokerConnectionString);

                var connection = Connection.Factory.CreateAsync(address)
                    .GetAwaiter()
                    .GetResult();

                return new Session((Connection)connection);
            })
            .AddTransient<IConsumer>(services => 
            {
                var logger = services.GetRequiredService<ILogger<AmqpConsumer>>();

                var session = services.GetRequiredService<ISession>();

                return new AmqpConsumer(logger, session);
            })
            .AddSingleton<IPublisher>(services => {
                var serializer = services.GetRequiredService<ISerializer>();

                var session = services.GetRequiredService<ISession>();

                return new AmqpPublisher(session, serializer);
            });
    }
}
