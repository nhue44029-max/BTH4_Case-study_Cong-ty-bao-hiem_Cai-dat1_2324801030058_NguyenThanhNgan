namespace ChatService.Test;

public class ProjectSetupTests
{
    [Fact]
    public void StatusDto_PreservesValues()
    {
        var status = new ChatService.Api.ChatServiceStatusDto("ChatService", "Healthy");

        Assert.Equal("ChatService", status.ServiceName);
        Assert.Equal("Healthy", status.Status);
    }
}
