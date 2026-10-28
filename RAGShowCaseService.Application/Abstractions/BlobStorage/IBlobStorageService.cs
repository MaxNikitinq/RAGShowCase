namespace RAGShowCaseService.Application.Abstractions.Storage;

public interface IBlobStorageService
{
    Task UploadFileAsync(Stream fileStream, string fileName, string containerName, CancellationToken cancellationToken = default);
}
