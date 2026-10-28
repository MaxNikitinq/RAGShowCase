using Microsoft.Extensions.DependencyInjection;
using RAGShowCaseService.Application.Features.AugmentedChat.Services;
using RAGShowCaseService.Application.Features.AugmentedChat.Services.Abstractions;
using RAGShowCaseService.Application.Features.Documents.Services.Abstractions;
using RAGShowCaseService.Application.Features.PrivateDocuments.Services;

namespace RAGShowCaseService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPrivateDocumentsService, PrivateDocumentsService>();
        services.AddScoped<IAugmentedChatService, AugmentedChatService>();

        return services;
    }
}
