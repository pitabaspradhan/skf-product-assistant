using Microsoft.SemanticKernel;
using Skf.ProductAssistant.Plugins;

namespace Skf.ProductAssistant.Services;

public static class SemanticKernelFactory
{
    public static Kernel CreateKernel()
    {
        var builder = Kernel.CreateBuilder();

        // 🔌 Azure OpenAI Connector
        builder.AddAzureOpenAIChatCompletion(
            deploymentName: Environment.GetEnvironmentVariable("AOAI_DEPLOYMENT"),
            endpoint: Environment.GetEnvironmentVariable("AOAI_ENDPOINT"),
            apiKey: Environment.GetEnvironmentVariable("AOAI_KEY"),
            apiVersion: "2024-12-01-preview");

        var kernel = builder.Build();

        // 🔧 Register Datasheet Plugin
        kernel.ImportPluginFromObject(
            new DatasheetPlugin(),
            "DatasheetPlugin");

        return kernel;
    }
}
