using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace RAGShowCaseService.Api.Controllers;

public abstract class ControllerBaseExtended : ControllerBase
{
    private readonly ProblemDetailsFactory _problemDetailsFactory;

    public ControllerBaseExtended(ProblemDetailsFactory problemDetailsFactory)
    {
        _problemDetailsFactory = problemDetailsFactory;
    }

    protected ProblemDetails? GenerateProblemDetails(string message, int statusCode)
    {
        var problemDetails = _problemDetailsFactory.CreateProblemDetails(HttpContext, statusCode);
        problemDetails.Detail = message;
        return problemDetails;
    }
}
