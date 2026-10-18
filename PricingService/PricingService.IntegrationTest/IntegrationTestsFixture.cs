using System.Threading.Tasks;
using Alba;
using Xunit;

namespace PricingService.IntegrationTest;

public class IntegrationTestsFixture : IAsyncLifetime
{
    public IAlbaHost SystemUnderTest { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        // The current PricingService stores its sample tariffs in memory. The HTTP test
        // exercises the real application pipeline without starting an unused database.
        SystemUnderTest = await AlbaHost.For<global::Program>();
    }

    public async Task DisposeAsync()
    {
        await SystemUnderTest.DisposeAsync();
    }
}
