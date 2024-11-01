using System.Reflection;
using Cypherly.Common.Messaging.Messages.PublishMessages;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Cypherly.MassTransit.Messaging.Configuration;

public static class MassTransitConfiguration
{
    /// <summary>
    /// Configures Masstransit with RabbitMQ, including retry and circut breaker policies.
    /// Optionally allows configuring sagas or aditinal components
    /// </summary>
    /// <param name="services">The service collection to add Masstransit to</param>
    /// <param name="consumerAssembly">The assembly containing consumers to register</param>
    /// <param name="configureAddtional">Optional configuration for adding sagas or additional MassTransit Components</param>
    /// <returns>The ServiceCollection <see cref="ServiceCollection"/></returns>
    /// <exception cref="InvalidOperationException">Exception thrown if RabbitMq <see cref="RabbitMqSettings"/> settings aren't configured; resulting in missing values for connection</exception>
    public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services, Assembly consumerAssembly, Action<IBusRegistrationConfigurator>? configureAddtional = null, Action<IRabbitMqBusFactoryConfigurator, IBusRegistrationContext>? configureRabbitMqAdditional = null)
    {
        services.AddMassTransit(x =>
        {
            configureAddtional?.Invoke(x);

            x.AddConsumers(consumerAssembly);

            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqSettings = context.GetRequiredService<IOptions<RabbitMqSettings>>().Value;

                cfg.Host(rabbitMqSettings.Host, "/", h =>
                {
                    h.Username(rabbitMqSettings.Username ?? throw new InvalidOperationException("Cannot initialize RabbitMQ without a username"));
                    h.Password(rabbitMqSettings.Password ?? throw new InvalidOperationException("Cannot initialize RabbitMQ without a password"));
                });


                cfg.UseMessageRetry(r=> r.Interval(5, TimeSpan.FromSeconds(5)));

                cfg.UseCircuitBreaker(cb =>
                {
                    cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                    cb.TripThreshold = 15;
                    cb.ActiveThreshold = 10;
                    cb.ResetInterval = TimeSpan.FromMinutes(5);
                });
                cfg.ConfigureEndpoints(context);
                configureRabbitMqAdditional?.Invoke(cfg, context);
            });
        });
        return services;
    }

    /// <summary>
    /// Add a <see cref="Producer{TMessage}"/> for a specific message type to the service collection
    /// </summary>
    /// <param name="services">the collection producer will be added to</param>
    /// <typeparam name="TMessage">the type the producer will handle. TMessage type should be of type <see cref="BaseMessage"/></typeparam>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddProducer<TMessage>(this IServiceCollection services) where TMessage : BaseMessage
    {
        services.AddScoped<IProducer<TMessage>, Producer<TMessage>>();
        return services;
    }
}