using RAGShowCaseService.Application.Abstractions.QueueStorage.Messages;

namespace RAGShowCaseService.Application.Abstractions.QueueStorage;

public interface IQueueStorageService
{
    Task SendMessageAsync(BaseMessage message, string queueName, CancellationToken cancellationToken = default);
}
