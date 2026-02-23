using StackExchange.Redis;
using System.Text.Json;

namespace Skf.ProductAssistant.State;

public class RedisStateStore : IStateStore
{
    private readonly IDatabase _db;

    public RedisStateStore()
    {
        var redis =
            ConnectionMultiplexer.Connect(
                Environment.GetEnvironmentVariable(
                    "REDIS_CONNECTION"));

        _db = redis.GetDatabase();
    }

    public async Task<ConversationState> GetAsync(string id)
    {
        var data = await _db.StringGetAsync(id);

        if (data.IsNullOrEmpty)
            return new ConversationState
            {
                ConversationId = id
            };

        // Explicitly convert RedisValue to string for deserialization
        return JsonSerializer.Deserialize<ConversationState>(data.ToString());
    }

    public async Task SaveAsync(ConversationState state)
    {
        var json =
            JsonSerializer.Serialize(state);

        await _db.StringSetAsync(
            state.ConversationId,
            json);
    }
}
