using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace RAGShowCaseService.DocumentProcessor;

public class DocumentProcessorFunction
{
    private readonly ILogger<DocumentProcessorFunction> _logger;
    private readonly DocumentProcessor _documentProcessor;

    public DocumentProcessorFunction(ILogger<DocumentProcessorFunction> logger, DocumentProcessor documentProcessor)
    {
        _logger = logger;
        _documentProcessor = documentProcessor;
    }

    [Function(nameof(DocumentProcessorFunction))]
    public async Task Run([QueueTrigger("rag-documents-process")] QueueMessage message)
    {
        var documentId = default(int);

        try
        {
            _logger.LogInformation("Received new document process message: id = {0} {1}", message.MessageId, message.InsertedOn);

            if (string.IsNullOrEmpty(message.MessageText))
            {
                return;
            }

            var payload = JsonSerializer.Deserialize<RAGDocumentProcessMessage>(message.MessageText);

            if (payload == null)
            {
                _logger.LogWarning("Can't deserialize message content. Invalid format");
                return;
            }

            _logger.LogInformation("Message payload: id = {0} {1}", payload.DocumentId, payload.DocumentName);

            await _documentProcessor.ProcessAsync($"{payload.DocumentId}_{payload.DocumentName}", payload.DocumentId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Document process failed: id = {0}", message.MessageId);

            if (documentId != default)
            {
                await _documentProcessor.UpdateDocumentStatus(documentId, Models.PrivateDocumentStatus.Failed);
            }

            throw new Exception(ex.Message);
        }
    }
}

public class RAGDocumentProcessMessage
{
    public int DocumentId { get; set; }

    public required string DocumentName { get; set; }

    public DateTime CreatedAt { get; set; }
}