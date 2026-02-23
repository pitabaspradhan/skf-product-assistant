using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Skf.ProductAssistant.Agents;
using Skf.ProductAssistant.Orchestration;
using Skf.ProductAssistant.Plugins;
using Skf.ProductAssistant.State;

var host = new HostBuilder()

    // 🌐 ASP.NET Core integration (required)
    .ConfigureFunctionsWebApplication()

    // 📘 Enable Swagger / OpenAPI
    .ConfigureOpenApi()

    // 🔌 Dependency Injection
    .ConfigureServices(services =>
    {
        // State Store
        services.AddSingleton<IStateStore, HybridStateStore>();

        // Plugin
        services.AddSingleton<DatasheetPlugin>();

        // Agents
        services.AddSingleton<QaAgent>();
        services.AddSingleton<FeedbackAgent>();

        // Orchestrator
        services.AddSingleton<IntentOrchestrator>();
    })

    .Build();

host.Run();
