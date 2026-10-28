using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using RAGShowCaseService.Application.Features.Documents.Services.Abstractions;
using System.Net.Mime;

namespace RAGShowCaseService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrivateDocumentsController : ControllerBaseExtended
    {
        private readonly Dictionary<string, string> _allowedTypes;

        private readonly IPrivateDocumentsService _privateDocumentsService;

        public PrivateDocumentsController(ProblemDetailsFactory problemDetailsFactory, IPrivateDocumentsService privateDocumentsService)
            : base(problemDetailsFactory)
        {
            _allowedTypes = new Dictionary<string, string>
            {
                { ".pdf", MediaTypeNames.Application.Pdf },
            };

            _privateDocumentsService = privateDocumentsService;
        }

        [HttpPost]
        [RequestSizeLimit(31457280)]            // 30 MB
        public async Task<IActionResult> Upload(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(GenerateProblemDetails("No file uploaded", StatusCodes.Status400BadRequest));
            }

            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!_allowedTypes.TryGetValue(extension, out var allowedMimeType))
            {
                return BadRequest(GenerateProblemDetails("Invalid MIME type", StatusCodes.Status400BadRequest));
            }

            if (!string.Equals(allowedMimeType, file.ContentType, StringComparison.InvariantCultureIgnoreCase))
            {
                return BadRequest(GenerateProblemDetails("Mismatch between MIME type and file extension", StatusCodes.Status400BadRequest));
            }

            using Stream fileStream = file.OpenReadStream();

            await _privateDocumentsService.UploadDocumentAsync(fileStream, file.FileName, cancellationToken);

            return NoContent();
        }

        [HttpPost("{documentId:int}/reprocess")]
        public async Task<IActionResult> Reprocess([FromRoute] int documentId, CancellationToken cancellationToken)
        {
            await _privateDocumentsService.ReprocessDocumentAsync(documentId, cancellationToken);

            return Accepted();
        }
    }
}
