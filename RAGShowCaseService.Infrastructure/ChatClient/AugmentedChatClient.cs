using Microsoft.Extensions.AI;
using RAGShowCaseService.Application.Abstractions.ChatClient;

namespace RAGShowCaseService.Infrastructure.ChatClient;

internal class AugmentedChatClient : IAugmentedChatClient
{
    private readonly IChatClient _chatClient;

    public AugmentedChatClient(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<string> GetResponseAsync(string question, string context, CancellationToken cancellationToken = default)
    {
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, $"""
                Answer the user's question based ONLY on the following context.
                If the answer is not in the context, say "I don't have information
                about that in my knowledge base."

                Context: {context}
                """),
            new ChatMessage(ChatRole.User, question)
        };

        var response = await _chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);

        return response.Text;
    }
}
