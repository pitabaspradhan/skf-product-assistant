using Microsoft.SemanticKernel;
using Skf.ProductAssistant.Agents;
using Skf.ProductAssistant.Services;
using Skf.ProductAssistant.State;

namespace Skf.ProductAssistant.Orchestration;

public class IntentOrchestrator
{
    private readonly Kernel? _kernel;
    private readonly QaAgent _qaAgent;
    private readonly FeedbackAgent _feedbackAgent;
    private readonly IStateStore _stateStore;

    public IntentOrchestrator(
        QaAgent qaAgent,
        FeedbackAgent feedbackAgent,
        IStateStore stateStore)
    {
        // Try to create kernel (may fail if offline)
        try
        {
            _kernel = SemanticKernelFactory.CreateKernel();
        }
        catch
        {
            _kernel = null;
        }

        _qaAgent = qaAgent;
        _feedbackAgent = feedbackAgent;
        _stateStore = stateStore;
    }

    public async Task<string> HandleAsync(
        string conversationId,
        string message)
    {
        var state =
            await _stateStore.GetAsync(conversationId);

        var intent =
            await ClassifyIntentAsync(message);

        if (intent == "feedback")
        {
            return await _feedbackAgent
                .HandleAsync(message, state);
        }

        return await _qaAgent
            .HandleAsync(message, state);
    }

    private async Task<string> ClassifyIntentAsync(
        string input)
    {
        // 🟡 Fallback if AI unavailable
        if (_kernel == null)
            return RuleBasedIntent(input);

        try
        {
            var prompt = """
            Classify the user input as:

            - question
            - feedback

            Return ONLY one word.

            Input: {{$input}}
            """;

            var result =
                await _kernel.InvokePromptAsync(
                    prompt,
                    new() { ["input"] = input });

            return result.ToString()
                         .Trim()
                         .ToLower();
        }
        catch
        {
            return RuleBasedIntent(input);
        }
    }

    // 🧠 Mock classifier
    private string RuleBasedIntent(string msg)
    {
        msg = msg.ToLower();

        if (msg.Contains("wrong") ||
            msg.Contains("feedback") ||
            msg.Contains("correction") ||
            msg.Contains("not correct"))
        {
            return "feedback";
        }

        return "question";
    }
}
