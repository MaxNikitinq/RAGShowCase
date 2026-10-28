using Pgvector;
using RAGShowCaseService.Application.Abstractions.ChatClient;
using RAGShowCaseService.Application.Abstractions.Embeddings;
using RAGShowCaseService.Application.Features.AugmentedChat.Services.Abstractions;
using RAGShowCaseService.Core.Repositories;

namespace RAGShowCaseService.Application.Features.AugmentedChat.Services;

internal class AugmentedChatService : IAugmentedChatService
{
    private readonly IAugmentedChatClient _augmentedChatClient;
    private readonly IEmbeddingsGenerator _embeddingsGenerator;
    private readonly IPrivateDocumentEmbeddingRepository _documentEmbeddingRepository;

    public AugmentedChatService(IAugmentedChatClient augmentedChatClient,
                                IEmbeddingsGenerator embeddingsGenerator,
                                IPrivateDocumentEmbeddingRepository documentEmbeddingRepository)
    {
        _augmentedChatClient = augmentedChatClient;
        _embeddingsGenerator = embeddingsGenerator;
        _documentEmbeddingRepository = documentEmbeddingRepository;
    }

    public async Task<string> AskAsync(string question, CancellationToken cancellationToken = default)
    {
        var questionEmbedding = await _embeddingsGenerator.GenerateAsync(question, cancellationToken);

        var results = await _documentEmbeddingRepository.SearchByCosineDistanceAsync(new Vector(questionEmbedding), 5, cancellationToken);

        var context = string.Join("\n\n", results);

        var response = await _augmentedChatClient.GetResponseAsync(question, context, cancellationToken);

        return response;
    }
}
