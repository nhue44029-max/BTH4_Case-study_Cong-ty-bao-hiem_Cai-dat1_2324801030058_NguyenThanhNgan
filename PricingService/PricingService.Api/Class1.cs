namespace PricingService.Api;

public interface IPricingServiceContract
{
    string ServiceName { get; }
}

public sealed record PricingServiceStatusDto(string ServiceName, string Status);
