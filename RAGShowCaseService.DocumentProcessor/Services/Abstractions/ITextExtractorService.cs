namespace RAGShowCaseService.DocumentProcessor.Services.Abstractions;

public interface ITextExtractorService
{
    string Extract(Stream documentStream);
}
