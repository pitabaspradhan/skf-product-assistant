using Skf.ProductAssistant.State;

public class MemoryStateStore : IStateStore
{
    private static readonly Dictionary<string, ConversationState> Store = new();

    public Task<ConversationState> GetAsync(string id)
    {
        if (!Store.ContainsKey(id))
            Store[id] = new ConversationState
            {
                ConversationId = id
            };

        return Task.FromResult(Store[id]);
    }

    public Task SaveAsync(ConversationState state)
    {
        Store[state.ConversationId] = state;
        return Task.CompletedTask;
    }
}
