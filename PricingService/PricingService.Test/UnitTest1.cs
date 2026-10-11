namespace PricingService.Test;

public class ProjectSetupTests
{
    [Fact]
    public void StatusDto_PreservesValues()
    {
        var status = new PricingService.Api.PricingServiceStatusDto("PricingService", "Healthy");

        Assert.Equal("PricingService", status.ServiceName);
        Assert.Equal("Healthy", status.Status);
    }
}
