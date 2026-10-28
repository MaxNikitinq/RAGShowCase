namespace RAGShowCaseService.Application.Abstractions.ChatClient;

public interface IAugmentedChatClient
{
    Task<string> GetResponseAsync(string question, string context, CancellationToken cancellationToken = default);
}
