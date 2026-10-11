namespace AuthService.Test;

public class ProjectSetupTests
{
    [Fact]
    public void StatusDto_PreservesValues()
    {
        var status = new AuthService.Api.AuthServiceStatusDto("AuthService", "Healthy");

        Assert.Equal("AuthService", status.ServiceName);
        Assert.Equal("Healthy", status.Status);
    }
}
