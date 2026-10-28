using Azure.Core;
using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OllamaSharp;
using Polly;
using RAGShowCaseService.Application.Abstractions.ChatClient;
using RAGShowCaseService.Application.Abstractions.Embeddings;
using RAGShowCaseService.Application.Abstractions.QueueStorage;
using RAGShowCaseService.Application.Abstractions.Storage;
using RAGShowCaseService.Application.Abstractions.UnitOfWork;
using RAGShowCaseService.Core.Repositories;
using RAGShowCaseService.Infrastructure.ChatClient;
using RAGShowCaseService.Infrastructure.Data;
using RAGShowCaseService.Infrastructure.Data.Repositories;
using RAGShowCaseService.Infrastructure.Embeddings;
using RAGShowCaseService.Infrastructure.QueueStorage;
using RAGShowCaseService.Infrastructure.Storage;

namespace RAGShowCaseService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(configuration.GetConnectionString("AzureBlobStorage"))
                         .ConfigureOptions(options =>
                         {
                             options.Retry.Mode = RetryMode.Exponential;
                             options.Retry.MaxRetries = 3;
                             options.Retry.Delay = TimeSpan.FromSeconds(1);
                             options.Retry.MaxDelay = TimeSpan.FromSeconds(5);
                         });

            clientBuilder.AddQueueServiceClient(configuration.GetConnectionString("AzureQueueStorage"))
                         .ConfigureOptions(options =>
                         {
                            options.MessageEncoding = QueueMessageEncoding.Base64;
                         });
        });

        services.AddHttpClient("OllamaClient")
                .AddStandardResilienceHandler(options =>
                {
                    options.Retry.BackoffType = DelayBackoffType.Exponential;
                    options.Retry.MaxRetryAttempts = 3;
                    options.Retry.Delay = TimeSpan.FromSeconds(1);
                    options.Retry.MaxDelay = TimeSpan.FromSeconds(10);
                });

        services.AddScoped<IEmbeddingGenerator<string, Embedding<float>>>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("OllamaClient");

            httpClient.BaseAddress = new Uri(configuration["Ollama:Url"]!);

            return new OllamaApiClient(httpClient, configuration["Ollama:EmbeddingModel"]!);
        });

        services.AddChatClient(new OllamaApiClient(new Uri(configuration["Ollama:Url"]!), configuration["Ollama:ChatModel"]!));

        // Enable vector support on the Npgsql driver
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreConnection"), opt => opt.UseVector()));

        services.AddScoped<IUnitOfWork>(x => x.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IBlobStorageService, AzureBlobStorageService>();
        services.AddScoped<IQueueStorageService, QueueStorageService>();
        services.AddScoped<IEmbeddingsGenerator, EmbeddingsGenerator>();
        services.AddScoped<IAugmentedChatClient, AugmentedChatClient>();

        services.AddScoped<IPrivateDocumentRepository, PrivateDocumentRepository>();
        services.AddScoped<IPrivateDocumentEmbeddingRepository, PrivateDocumentEmbeddingRepository>();

        return services;
    }
}
