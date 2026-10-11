namespace PolicyService.Test;

public class ProjectSetupTests
{
    [Fact]
    public void StatusDto_PreservesValues()
    {
        var status = new PolicyService.Api.PolicyServiceStatusDto("PolicyService", "Healthy");

        Assert.Equal("PolicyService", status.ServiceName);
        Assert.Equal("Healthy", status.Status);
    }
}
