using FluentValidation;
using PricingService.Data;
using PricingService.Domain;
using Steeltoe.Discovery.Client;

namespace PricingService;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers().AddNewtonsoftJson();
        services.AddHealthChecks();
        services.AddDiscoveryClient(Configuration);
        services.AddPricingServiceData(Configuration);
        services.AddSingleton<IDataStore, InMemoryTariffDataStore>();
        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(typeof(Program).Assembly));
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(WebApplication app, IWebHostEnvironment env)
    {
        app.Use(async (context, next) =>
        {
            try
            {
                await next();
            }
            catch (ValidationException exception)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new
                {
                    errors = exception.Errors.Select(error => new { error.PropertyName, error.ErrorMessage })
                });
            }
        });

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();
        app.MapControllers();
        app.MapHealthChecks("/health");
    }
}
