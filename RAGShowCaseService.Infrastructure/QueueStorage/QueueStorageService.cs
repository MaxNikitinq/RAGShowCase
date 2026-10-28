using Azure.Storage.Queues;
using RAGShowCaseService.Application.Abstractions.QueueStorage;
using RAGShowCaseService.Application.Abstractions.QueueStorage.Messages;
using System.Text.Json;

namespace RAGShowCaseService.Infrastructure.QueueStorage;

internal class QueueStorageService : IQueueStorageService
{
    private readonly QueueServiceClient _queueServiceClient;

    public QueueStorageService(QueueServiceClient queueServiceClient)
    {
        _queueServiceClient = queueServiceClient;
    }

    public async Task SendMessageAsync(BaseMessage message, string queueName, CancellationToken cancellationToken = default)
    {
        var queueClient = _queueServiceClient.GetQueueClient(queueName);

        await queueClient.CreateIfNotExistsAsync();

        var jsonMessage = JsonSerializer.Serialize(message);

        await queueClient.SendMessageAsync(jsonMessage, cancellationToken);
    }
}
