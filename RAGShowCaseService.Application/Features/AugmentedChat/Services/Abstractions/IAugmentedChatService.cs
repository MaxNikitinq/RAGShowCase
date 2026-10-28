namespace RAGShowCaseService.Application.Features.AugmentedChat.Services.Abstractions;

public interface IAugmentedChatService
{
    Task<string> AskAsync(string promt, CancellationToken cancellationToken = default);
}
