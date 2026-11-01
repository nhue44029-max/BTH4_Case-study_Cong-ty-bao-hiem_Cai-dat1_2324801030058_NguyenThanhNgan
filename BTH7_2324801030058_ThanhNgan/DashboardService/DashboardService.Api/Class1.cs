namespace DashboardService.Api;

public interface IDashboardServiceContract
{
    string ServiceName { get; }
}

public sealed record DashboardServiceStatusDto(string ServiceName, string Status);
