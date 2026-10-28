using Azure.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using OllamaSharp;
using Polly;
using RAGShowCaseService.DocumentProcessor;
using RAGShowCaseService.DocumentProcessor.Commands;
using RAGShowCaseService.DocumentProcessor.Services;
using RAGShowCaseService.DocumentProcessor.Services.Abstractions;
using System.Data.Common;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = FunctionsApplication.CreateBuilder(args);

        builder.ConfigureFunctionsWebApplication();

        builder.Services
            .AddApplicationInsightsTelemetryWorkerService()
            .ConfigureFunctionsApplicationInsights();

        builder.Services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(builder.Configuration.GetConnectionString("AzureBlobStorage"))
                         .ConfigureOptions(options =>
                         {
                             options.Retry.Mode = RetryMode.Exponential;
                             options.Retry.MaxRetries = 3;
                             options.Retry.Delay = TimeSpan.FromSeconds(1);
                             options.Retry.MaxDelay = TimeSpan.FromSeconds(10);
                         });
        });

        builder.Services.AddHttpClient("OllamaClient")
                        .AddStandardResilienceHandler(options =>
                        {
                            options.Retry.BackoffType = DelayBackoffType.Exponential;
                            options.Retry.MaxRetryAttempts = 3;
                            options.Retry.Delay = TimeSpan.FromSeconds(1);
                            options.Retry.MaxDelay = TimeSpan.FromSeconds(10);
                        });

        builder.Services.AddScoped<IEmbeddingGenerator<string, Embedding<float>>>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("OllamaClient");

            httpClient.BaseAddress = new Uri(builder.Configuration["Ollama:Url"]!);

            return new OllamaApiClient(httpClient, builder.Configuration["Ollama:EmbeddingModel"]!);
        });

        builder.Services.AddTransient<DbConnection>((sp) =>
            new NpgsqlConnection(builder.Configuration.GetConnectionString("PostgreConnection")));

        builder.Services.AddScoped<ITextExtractorService, TextExtractorService>();
        builder.Services.AddScoped<IChunkerService, ChunkerService>();
        builder.Services.AddScoped<DocumentProcessor>();

        builder.Services.AddScoped<IInsertDocumentEmbeddingsCommand, InsertDocumentEmbeddingsCommand>();
        builder.Services.AddScoped<IUpdateDocumentStatusCommand, UpdateDocumentStatusCommand>();
        builder.Services.AddScoped<IResetPrivateDocumentCommand, ResetPrivateDocumentCommand>();

        builder.Build().Run();
    }
}