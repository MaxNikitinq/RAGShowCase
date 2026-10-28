namespace RAGShowCaseService.Application.Abstractions.QueueStorage.Messages;

public sealed record RAGDocumentProcessMessage(int DocumentId, string DocumentName, DateTime CreatedAt) : BaseMessage(CreatedAt);

