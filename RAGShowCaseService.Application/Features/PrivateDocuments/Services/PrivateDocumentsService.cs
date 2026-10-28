using RAGShowCaseService.Application.Abstractions.BlobStorage;
using RAGShowCaseService.Application.Abstractions.QueueStorage;
using RAGShowCaseService.Application.Abstractions.QueueStorage.Messages;
using RAGShowCaseService.Application.Abstractions.Storage;
using RAGShowCaseService.Application.Abstractions.UnitOfWork;
using RAGShowCaseService.Application.Features.Documents.Services.Abstractions;
using RAGShowCaseService.Core.Entities;
using RAGShowCaseService.Core.Repositories;

namespace RAGShowCaseService.Application.Features.PrivateDocuments.Services;

internal class PrivateDocumentsService : IPrivateDocumentsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPrivateDocumentRepository _privateDocumentRepository;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IQueueStorageService _queueStorageService;

    public PrivateDocumentsService(IUnitOfWork unitOfWork,
                                   IPrivateDocumentRepository privateDocumentRepository,
                                   IBlobStorageService blobStorageService,
                                   IQueueStorageService queueStorageService)
    {
        _unitOfWork = unitOfWork;
        _privateDocumentRepository = privateDocumentRepository;
        _blobStorageService = blobStorageService;
        _queueStorageService = queueStorageService;
    }

    public async Task UploadDocumentAsync(Stream stream, string name, CancellationToken cancellationToken = default)
    {
        var newDocument = new PrivateDocument(name);

        await _privateDocumentRepository.AddAsync(newDocument, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _blobStorageService.UploadFileAsync(stream, $"{newDocument.Id}_{name}", BlobStorageContainers.RAGDocuments, cancellationToken);

            newDocument.UpdateStatus(PrivateFileStatus.Uploaded);

            await _unitOfWork.SaveChangesAsync();

            await _queueStorageService.SendMessageAsync(new RAGDocumentProcessMessage(newDocument.Id, newDocument.Name, DateTime.UtcNow), Queues.RAGDocumentsProcess, cancellationToken);
        }
        catch (Exception)
        {
            // failed to upload document to blob or cancellation was requested
            // so need to cleanup db record
            newDocument.Deactivate();

            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task ReprocessDocumentAsync(int documentId, CancellationToken cancellationToken = default)
    {
        var document = await _privateDocumentRepository.GetAsync(documentId, cancellationToken);

        if (document == null)
        {
            throw new Exception("Document not found");
        }

        document.UpdateStatus(PrivateFileStatus.Uploaded);

        await _unitOfWork.SaveChangesAsync();

        await _queueStorageService.SendMessageAsync(new RAGDocumentProcessMessage(document.Id, document.Name, DateTime.UtcNow), Queues.RAGDocumentsProcess, cancellationToken);
    }
}
