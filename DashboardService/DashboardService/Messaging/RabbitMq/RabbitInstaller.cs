using System;
using System.Collections.Generic;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DashboardService.Messaging.RabbitMq;

public static class RabbitInstaller
{
    public static IServiceCollection AddRabbitListeners(this IServiceCollection services, RabbitMqOptions options, string connectionString)
    {
        var host = options.Host;
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