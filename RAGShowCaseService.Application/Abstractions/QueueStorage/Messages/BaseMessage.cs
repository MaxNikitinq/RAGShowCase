using System.Text.Json.Serialization;

namespace RAGShowCaseService.Application.Abstractions.QueueStorage.Messages;

[JsonDerivedType(typeof(RAGDocumentProcessMessage))]
public abstract record BaseMessage(DateTime CreatedAt);

