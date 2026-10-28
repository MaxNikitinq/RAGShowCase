namespace RAGShowCaseService.Application.Features.Documents.Services.Abstractions;

public interface IPrivateDocumentsService
{
    Task UploadDocumentAsync(Stream stream, string name, CancellationToken cancellationToken = default);

    Task ReprocessDocumentAsync(int documentId, CancellationToken cancellationToken = default);
}
