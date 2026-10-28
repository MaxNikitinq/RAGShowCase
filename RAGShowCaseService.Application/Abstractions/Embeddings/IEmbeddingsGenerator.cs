namespace RAGShowCaseService.Application.Abstractions.Embeddings;

public interface IEmbeddingsGenerator
{
    Task<float[]> GenerateAsync(string text, CancellationToken cancellationToken = default);
}