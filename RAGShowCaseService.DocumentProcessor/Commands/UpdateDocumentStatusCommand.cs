using Dapper;
using RAGShowCaseService.DocumentProcessor.Models;
using System.Data.Common;

namespace RAGShowCaseService.DocumentProcessor.Commands;

public interface IUpdateDocumentStatusCommand
{
    Task ExecuteAsync(int documentId, PrivateDocumentStatus status);
}

internal class UpdateDocumentStatusCommand : IUpdateDocumentStatusCommand
{
    private readonly DbConnection _dbConnection;

    public UpdateDocumentStatusCommand(DbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task ExecuteAsync(int documentId, PrivateDocumentStatus status)
    {
        var sql = @"
            UPDATE private_documents
            SET status = @status
            WHERE id = @documentId";

        await _dbConnection.ExecuteAsync(sql, new { documentId, status = status.ToString() });
    }
}
