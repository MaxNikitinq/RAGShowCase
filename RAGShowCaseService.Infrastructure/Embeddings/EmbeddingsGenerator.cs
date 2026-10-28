using Microsoft.Extensions.AI;
using RAGShowCaseService.Application.Abstractions.Embeddings;

namespace RAGShowCaseService.Infrastructure.Embeddings;

internal class EmbeddingsGenerator : IEmbeddingsGenerator
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;

    public EmbeddingsGenerator(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
    {
        _embeddingGenerator = embeddingGenerator;
    }

    public async Task<float[]> GenerateAsync(string text, CancellationToken cancellationToken = default)
    {
        return (await _embeddingGenerator.GenerateVectorAsync(text, cancellationToken: cancellationToken)).ToArray();
    }
}
