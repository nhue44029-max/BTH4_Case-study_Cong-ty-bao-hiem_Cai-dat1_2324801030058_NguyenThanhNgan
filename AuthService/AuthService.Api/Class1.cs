namespace AuthService.Api;

public interface IAuthServiceContract
{
    string ServiceName { get; }
}

public sealed record AuthServiceStatusDto(string ServiceName, string Status);
