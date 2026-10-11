namespace PolicyService.Api;

public interface IPolicyServiceContract
{
    string ServiceName { get; }
}

public sealed record PolicyServiceStatusDto(string ServiceName, string Status);
