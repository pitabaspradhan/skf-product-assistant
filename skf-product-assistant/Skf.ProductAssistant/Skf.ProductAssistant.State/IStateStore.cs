using Skf.ProductAssistant.State;

public interface IStateStore
{
    Task<ConversationState> GetAsync(string id);
    Task SaveAsync(ConversationState state);
}
