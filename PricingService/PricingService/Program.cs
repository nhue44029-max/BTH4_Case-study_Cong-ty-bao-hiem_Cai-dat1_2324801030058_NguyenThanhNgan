using Steeltoe.Discovery.Client;
using FluentValidation;
using MediatR;
using PricingService.Data;
using PricingService.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddHealthChecks();
builder.Services.AddDiscoveryClient();
builder.Services.AddPricingServiceData(builder.Configuration);
builder.Services.AddSingleton<IDataStore, InMemoryTariffDataStore>();
builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();

public partial class Program { }
