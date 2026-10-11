namespace DashboardService.Test;

public class ProjectSetupTests
{
    [Fact]
    public void StatusDto_PreservesValues()
    {
        var status = new DashboardService.Api.DashboardServiceStatusDto("DashboardService", "Healthy");

        Assert.Equal("DashboardService", status.ServiceName);
        Assert.Equal("Healthy", status.Status);
    }
}
