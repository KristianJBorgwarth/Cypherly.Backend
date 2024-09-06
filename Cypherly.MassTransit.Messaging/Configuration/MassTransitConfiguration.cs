using System.Reflection;
using Cypherly.Application.Contracts.Messaging.PublishMessages;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Cypherly.MassTransit.Messaging.Configuration;

public static class MassTransitConfiguration
{
    public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services, Assembly consumerAssembly)
    {
        services.AddMassTransit(x =>
        {
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
            });
        });
        return services;
    }

    /// <summary>
    /// Add a <see cref="Producer{TMessage}"/> for a specific message type to the service collection
    /// </summary>
    /// <param name="services">service collection producer will be added to</param>
    /// <typeparam name="TMessage">TMessage type the producer will handle. TMessage type should be of type <see cref="BaseMessage"/></typeparam>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddProducer<TMessage>(this IServiceCollection services) where TMessage : BaseMessage
    {
        services.AddScoped<IProducer<TMessage>, Producer<TMessage>>();
        return services;
    }
}