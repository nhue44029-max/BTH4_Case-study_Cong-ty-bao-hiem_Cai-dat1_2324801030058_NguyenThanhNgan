namespace DocumentService.Test;

public class ProjectSetupTests
{
    [Fact]
    public void StatusDto_PreservesValues()
    {
        var status = new DocumentService.Api.DocumentServiceStatusDto("DocumentService", "Healthy");

        Assert.Equal("DocumentService", status.ServiceName);
        Assert.Equal("Healthy", status.Status);
    }
}
