namespace PaymentService.Test;

public class ProjectSetupTests
{
    [Fact]
    public void StatusDto_PreservesValues()
    {
        var status = new PaymentService.Api.PaymentServiceStatusDto("PaymentService", "Healthy");

        Assert.Equal("PaymentService", status.ServiceName);
        Assert.Equal("Healthy", status.Status);
    }
}
