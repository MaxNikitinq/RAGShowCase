using RAGShowCaseService.DocumentProcessor.Models;
using RAGShowCaseService.DocumentProcessor.Services.Abstractions;
using System.Text.RegularExpressions;

namespace RAGShowCaseService.DocumentProcessor.Services;

internal class ChunkerService : IChunkerService
{
    private readonly int _chunkSize;
    private readonly int _chunkOverlap;

    public ChunkerService(int chunkSize = 500, int chunkOverlap = 100)
    {
        _chunkSize = chunkSize;
        _chunkOverlap = chunkOverlap;
    }

    public List<DocumentChunk> PrepareChunks(string documentContent, int documentId)
    {
        var sanitazedContent = SanitizeContent(documentContent);

        var chunks = new List<DocumentChunk>();
        var currrentIndex = 0;
        var currentChunkIndex = 0;

        while (currrentIndex < sanitazedContent.Length)
        {
            var startIndex = Math.Max(0, currrentIndex - _chunkOverlap);
            var endIndex = Math.Min(startIndex + _chunkSize, sanitazedContent.Length);
            var length = endIndex - startIndex;

            chunks.Add(new DocumentChunk
            {
                DocumentId = documentId,
                Text = sanitazedContent.Substring(startIndex, length),
                ChunkIndex = currentChunkIndex++,
            });

            currrentIndex = endIndex;
        }

        return chunks;
    }

    private string SanitizeContent(string documentContent)
    {
        if (string.IsNullOrWhiteSpace(documentContent))
        {
            return documentContent;
        }

        documentContent = Regex.Replace(documentContent, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", " ");
        documentContent = Regex.Replace(documentContent, @"[•·†‡°™®©℠€£¥¢ƒ¤§¶‖†‡‣⁎↓↑→←⇒⇐⇑⇓◄►★☆✓✗✘✔◇◆▪▫►◄║═╔╗╚╝╠╣╦╩╬├┤┬┴┼─│┌┐└┘├┤┬┴┼]", " ");
        documentContent = Regex.Replace(documentContent, @"https?://\S+|www\.\S+", " ");
        documentContent = Regex.Replace(documentContent, @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b", " ");
        documentContent = Regex.Replace(documentContent, @"[!?;:]{2,}", ".");
        documentContent = Regex.Replace(documentContent, @"\b([!?;:.,\-])+", "");
        documentContent = Regex.Replace(documentContent, @"([!?;:.,\-])+\b", "");
        documentContent = Regex.Replace(documentContent, @"[^a-zA-Z0-9\s\.\,\!\?\:\-\(\)\'\""]", " ");
        documentContent = Regex.Replace(documentContent, @"\s+", " ");
        documentContent = Regex.Replace(documentContent, @"\b[a-zA-Z0-9]\b\s*", " ");
        documentContent = documentContent.Trim();
        documentContent = Regex.Replace(documentContent, @"\s+", " ");

        return documentContent;
    }
}
