using Dapper;
using RAGShowCaseService.DocumentProcessor.Models;
using System.Data.Common;

namespace RAGShowCaseService.DocumentProcessor.Commands;

public interface IInsertDocumentEmbeddingsCommand
{
    Task ExecuteAsync(List<DocumentChunk> documentChunks);
}

internal class InsertDocumentEmbeddingsCommand : IInsertDocumentEmbeddingsCommand
{
    private readonly DbConnection _dbConnection;

    public InsertDocumentEmbeddingsCommand(DbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task ExecuteAsync(List<DocumentChunk> documentChunks)
    {
        var sql = @"
            INSERT INTO private_document_embeddings (private_document_Id, text, embedding, chunk_index)
            VALUES (@DocumentId, @Text, @Embedding, @ChunkIndex)";

        await _dbConnection.ExecuteAsync(sql, documentChunks);
    }
}
