namespace ProductService.Test;

public class ProjectSetupTests
{
    [Fact]
    public void StatusDto_PreservesValues()
    {
        var status = new ProductService.Api.ProductServiceStatusDto("ProductService", "Healthy");

        Assert.Equal("ProductService", status.ServiceName);
        Assert.Equal("Healthy", status.Status);
    }
}
