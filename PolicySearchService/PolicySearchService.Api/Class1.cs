namespace PolicySearchService.Api;

public interface IPolicySearchServiceContract
{
    string ServiceName { get; }
}

public sealed record PolicySearchServiceStatusDto(string ServiceName, string Status);
