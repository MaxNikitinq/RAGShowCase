using Dapper;
using System.Data.Common;

namespace RAGShowCaseService.DocumentProcessor.Commands;

public interface IResetPrivateDocumentCommand
{
    Task ExecuteAsync(int documentId);
}

internal class ResetPrivateDocumentCommand : IResetPrivateDocumentCommand
{
    private readonly DbConnection _dbConnection;

    public ResetPrivateDocumentCommand(DbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task ExecuteAsync(int documentId)
    {
        var deleteEmbeddingsSql = @"
            DELETE FROM private_document_embeddings
            WHERE private_document_id = @documentId";

        await _dbConnection.ExecuteAsync(deleteEmbeddingsSql, new { documentId });
    }
}
