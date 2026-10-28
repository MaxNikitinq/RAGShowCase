using RAGShowCaseService.DocumentProcessor.Services.Abstractions;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace RAGShowCaseService.DocumentProcessor.Services;

internal class TextExtractorService : ITextExtractorService
{
    public string Extract(Stream documentStream)
    {
        var documentContent = new StringBuilder();

        var document = PdfDocument.Open(documentStream);
        var pages = document.GetPages();

        foreach (var page in pages)
        {
            var pageText = ContentOrderTextExtractor.GetText(page);

            documentContent.Append(pageText);
            documentContent.Append(" ");
        }

        return documentContent.ToString();

    }

    //private string SanitazeContent(string documentContent)
    //{
    //    if (string.IsNullOrWhiteSpace(documentContent))
    //    {
    //        return documentContent;
    //    }

    //    documentContent = Regex.Replace(documentContent, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", " ");
    //    documentContent = Regex.Replace(documentContent, @"[•·†‡°™®©℠€£¥¢ƒ¤§¶‖†‡‣⁎↓↑→←⇒⇐⇑⇓◄►★☆✓✗✘✔◇◆▪▫►◄║═╔╗╚╝╠╣╦╩╬├┤┬┴┼─│┌┐└┘├┤┬┴┼]", " ");
    //    documentContent = Regex.Replace(documentContent, @"https?://\S+|www\.\S+", " ");
    //    documentContent = Regex.Replace(documentContent, @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b", " ");
    //    documentContent = Regex.Replace(documentContent, @"[!?;:]{2,}", ".");
    //    documentContent = Regex.Replace(documentContent, @"\b([!?;:.,\-])+", "");
    //    documentContent = Regex.Replace(documentContent, @"([!?;:.,\-])+\b", "");
    //    documentContent = Regex.Replace(documentContent, @"[^a-zA-Z0-9\s\.\,\!\?\:\-\(\)\'\""]", " ");
    //    documentContent = Regex.Replace(documentContent, @"\s+", " ");
    //    documentContent = Regex.Replace(documentContent, @"\b[a-zA-Z0-9]\b\s*", " ");
    //    documentContent = documentContent.Trim();
    //    documentContent = Regex.Replace(documentContent, @"\s+", " ");

    //    return documentContent;
    //}
}
