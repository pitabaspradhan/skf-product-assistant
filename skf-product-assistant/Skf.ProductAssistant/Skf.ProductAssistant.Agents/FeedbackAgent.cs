using Skf.ProductAssistant.State;

namespace Skf.ProductAssistant.Agents;

public class FeedbackAgent
{
    public Task<string> HandleAsync(
        string message,
        ConversationState state)
    {
        // In real version → store in Redis

        var designation =
            state.LastDesignation ?? "unknown";

        var attribute =
            state.LastAttribute ?? "unknown";

        return Task.FromResult(
            $"Thanks — your feedback for {designation} / {attribute} has been saved.");
    }
}
