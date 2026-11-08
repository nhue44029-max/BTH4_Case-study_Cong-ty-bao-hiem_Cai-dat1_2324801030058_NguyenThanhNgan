using System;
using System.Collections.Generic;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ChatService.Messaging.RabbitMq;

public static class RabbitInstaller
{
    public static IServiceCollection AddRabbitListeners(this IServiceCollection services, string connectionString)
    {
        var host = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true" ? "rabbit" : "localhost";
        var bus = RabbitHutch.CreateBus(connectionString ?? $"host={host}:5672;username=guest;password=guest");
        bus.Advanced.ExchangeDeclare("lab-dotnet-micro", ExchangeType.Topic);
        services.AddSingleton(bus);
        
        services.AddSingleton(svc => new RabbitEventListener(svc.GetRequiredService<IBus>(), svc));

        return services;
    }
}

public static class RabbitListenersInstaller
{
    public static void UseRabbitListeners(this IApplicationBuilder app, List<Type> eventTypes)
    {
        app.ApplicationServices.GetRequiredService<RabbitEventListener>().ListenTo(eventTypes);
    }
}
