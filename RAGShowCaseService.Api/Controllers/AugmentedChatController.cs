using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using RAGShowCaseService.Application.Features.AugmentedChat.Services.Abstractions;

namespace RAGShowCaseService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AugmentedChatController : ControllerBaseExtended
{
    private readonly IAugmentedChatService _augmentedChatService;

    public AugmentedChatController(ProblemDetailsFactory problemDetailsFactory, IAugmentedChatService augmentedChatService)
        : base(problemDetailsFactory)
    {
        _augmentedChatService = augmentedChatService;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string searchText, CancellationToken cancellationToken = default)
    {
        var result = await _augmentedChatService.AskAsync(searchText, cancellationToken);

        return Ok(result);
    }
}
