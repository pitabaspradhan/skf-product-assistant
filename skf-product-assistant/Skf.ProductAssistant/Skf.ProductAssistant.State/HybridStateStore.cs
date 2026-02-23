using Skf.ProductAssistant.State;

public class HybridStateStore : IStateStore
{
    private readonly MemoryStateStore _memory;
    private readonly RedisStateStore _redis;

    public HybridStateStore()
    {
        _memory = new MemoryStateStore();

        try
        {
            _redis = new RedisStateStore();
        }
        catch
        {
            _redis = null;
        }
    }

    public async Task<ConversationState> GetAsync(string id)
    {
        if (_redis != null)
            return await _redis.GetAsync(id);

        return await _memory.GetAsync(id);
    }

    public async Task SaveAsync(ConversationState state)
    {
        if (_redis != null)
            await _redis.SaveAsync(state);
        else
            await _memory.SaveAsync(state);
    }
}
