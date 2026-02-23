using Microsoft.SemanticKernel;
using Skf.ProductAssistant.Services;
using Xunit;

public class SemanticKernelTests
{
    [Fact]
    public async Task Kernel_Should_Respond()
    {
        Environment.SetEnvironmentVariable(
            "AOAI_ENDPOINT",
            "https://developer-evals-foundry.cognitiveservices.azure.com/");

        Environment.SetEnvironmentVariable(
            "AOAI_KEY",
            "YOUR_KEY");

        Environment.SetEnvironmentVariable(
            "AOAI_DEPLOYMENT",
            "Pitabas_Pradhan");

        // 🔍 Debug prints
        Console.WriteLine("Endpoint: " +
            Environment.GetEnvironmentVariable("AOAI_ENDPOINT"));

        Console.WriteLine("Deployment: " +
            Environment.GetEnvironmentVariable("AOAI_DEPLOYMENT"));

        Console.WriteLine("Key exists: " +
            !string.IsNullOrEmpty(
                Environment.GetEnvironmentVariable("AOAI_KEY")));

        var kernel = SemanticKernelFactory.CreateKernel();

        var result = await kernel.InvokePromptAsync("Hello");

        Assert.False(string.IsNullOrWhiteSpace(result.ToString()));
    }
}
