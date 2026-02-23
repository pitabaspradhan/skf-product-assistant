using System.Text.Json;
using Microsoft.SemanticKernel;
using Skf.ProductAssistant.Models;
using Skf.ProductAssistant.Plugins;
using Skf.ProductAssistant.Services;
using Skf.ProductAssistant.State;

namespace Skf.ProductAssistant.Agents;

public class QaAgent
{
    private readonly Kernel? _kernel;
    private readonly DatasheetPlugin _plugin;

    public QaAgent(DatasheetPlugin plugin)
    {
        _plugin = plugin;

        try
        {
            _kernel = SemanticKernelFactory.CreateKernel();
        }
        catch
        {
            _kernel = null;
        }
    }

    public async Task<string> HandleAsync(
        string message,
        ConversationState state)
    {
        var query =
            await ExtractQueryAsync(message, state);

        string resultText;

        // 🔌 Function calling via SK
        if (_kernel != null)
        {
            var result =
                await _kernel.InvokeAsync(
                    "DatasheetPlugin",
                    "GetAttribute",
                    new()
                    {
                        ["designation"] = query.Designation,
                        ["attribute"] = query.Attribute
                    });

            resultText = result.ToString();
        }
        else
        {
            // 🟡 Fallback direct plugin call
            resultText =
                _plugin.GetAttribute(
                    query.Designation,
                    query.Attribute);
        }

        // 🧠 Update conversational state
        state.LastDesignation = query.Designation;
        state.LastAttribute = query.Attribute;
        state.LastAnswer = resultText;

        return resultText;
    }


    // 🧠 AI Extraction
    private async Task<ProductQuery>
        ExtractQueryAsync(
            string message,
            ConversationState state)
    {
        if (_kernel == null)
            return RuleBasedExtraction(message, state);

        try
        {
            var prompt = """
            Extract the product designation and attribute
            from the user query.

            Return JSON only:

            {
              "designation": "",
              "attribute": ""
            }

            Query: {{$input}}
            """;

            var result =
                await _kernel.InvokePromptAsync(
                    prompt,
                    new() { ["input"] = message });

            return JsonSerializer.Deserialize<ProductQuery>(
                result.ToString(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
        catch
        {
            return RuleBasedExtraction(message, state);
        }
    }

    // 🟡 Fallback extraction
    private ProductQuery RuleBasedExtraction(
        string message,
        ConversationState state)
    {
        message = message.ToLower();

        string designation =
            message.Contains("6205") ? "6205" :
            message.Contains("6025") ? "6025 N" :
            state.LastDesignation;

        string attribute =
            message.Contains("width") ? "width" :
            message.Contains("diameter") ? "diameter" :
            message.Contains("bore") ? "bore" :
            state.LastAttribute;

        return new ProductQuery
        {
            Designation = designation,
            Attribute = attribute
        };
    }
}
