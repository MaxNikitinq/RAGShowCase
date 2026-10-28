using Azure.Storage.Blobs;
using Microsoft.Extensions.AI;
using RAGShowCaseService.DocumentProcessor.Commands;
using RAGShowCaseService.DocumentProcessor.Models;
using RAGShowCaseService.DocumentProcessor.Services.Abstractions;

namespace RAGShowCaseService.DocumentProcessor;

public class DocumentProcessor
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    private readonly ITextExtractorService _textExtractorService;
    private readonly IChunkerService _chunkerService;
    private readonly IInsertDocumentEmbeddingsCommand _insertDocumentEmbeddingsCommand;
    private readonly IUpdateDocumentStatusCommand _updateDocumentStatusCommand;
    private readonly IResetPrivateDocumentCommand _resetPrivateDocumentCommand;

    private const string _blobContainerName = "rag-documents";

    public DocumentProcessor(BlobServiceClient blobServiceClient,
                             IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
                             ITextExtractorService textExtractorService,
                             IChunkerService chunkerService,
                             IInsertDocumentEmbeddingsCommand insertDocumentEmbeddingsCommand,
                             IUpdateDocumentStatusCommand updateDocumentStatusCommand,
                             IResetPrivateDocumentCommand resetPrivateDocumentCommand)
    {
        _blobServiceClient = blobServiceClient;
        _embeddingGenerator = embeddingGenerator;
        _textExtractorService = textExtractorService;
        _chunkerService = chunkerService;
        _insertDocumentEmbeddingsCommand = insertDocumentEmbeddingsCommand;
        _updateDocumentStatusCommand = updateDocumentStatusCommand;
        _resetPrivateDocumentCommand = resetPrivateDocumentCommand;
    }

    public async Task ProcessAsync(string documentPath, int documentId)
    {
        // since its allow to reprocess uploaded document
        // need to cleanup existing embeddings
        await _resetPrivateDocumentCommand.ExecuteAsync(documentId);

        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName);

        var blobClient = containerClient.GetBlobClient(documentPath);

        using var documentStream = await blobClient.OpenReadAsync();
        
        var documentContent = _textExtractorService.Extract(documentStream);
        var chunks = _chunkerService.PrepareChunks(documentContent, documentId);

        foreach (var chunk in chunks)
        {
            chunk.Embedding = (await _embeddingGenerator.GenerateVectorAsync(chunk.Text)).ToArray();
        }

        await _insertDocumentEmbeddingsCommand.ExecuteAsync(chunks);

        await UpdateDocumentStatus(documentId, PrivateDocumentStatus.Processed);
    }

    public async Task UpdateDocumentStatus(int documentId, PrivateDocumentStatus status)
    {
        await _updateDocumentStatusCommand.ExecuteAsync(documentId, status);
    }
}
