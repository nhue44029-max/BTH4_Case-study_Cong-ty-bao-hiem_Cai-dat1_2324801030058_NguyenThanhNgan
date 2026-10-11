namespace ProductService.Api;

public interface IProductServiceContract
{
    string ServiceName { get; }
}

public sealed record ProductServiceStatusDto(string ServiceName, string Status);
