namespace PolicySearchService.Test;

public class ProjectSetupTests
{
    [Fact]
    public void StatusDto_PreservesValues()
    {
        var status = new PolicySearchService.Api.PolicySearchServiceStatusDto("PolicySearchService", "Healthy");

        Assert.Equal("PolicySearchService", status.ServiceName);
        Assert.Equal("Healthy", status.Status);
    }
}
