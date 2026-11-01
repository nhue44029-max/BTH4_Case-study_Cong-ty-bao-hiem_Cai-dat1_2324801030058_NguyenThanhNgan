namespace ChatService.Api;

public interface IChatServiceContract
{
    string ServiceName { get; }
}

public sealed record ChatServiceStatusDto(string ServiceName, string Status);
