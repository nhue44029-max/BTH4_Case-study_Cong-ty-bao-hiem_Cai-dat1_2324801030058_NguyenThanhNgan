namespace DocumentService.Api;

public interface IDocumentServiceContract
{
    string ServiceName { get; }
}

public sealed record DocumentServiceStatusDto(string ServiceName, string Status);
