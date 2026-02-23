using System.Text.Json;
using Microsoft.SemanticKernel;

namespace Skf.ProductAssistant.Plugins;

public class DatasheetPlugin
{
    private readonly Dictionary<string, JsonElement> _datasheets;

    public DatasheetPlugin()
    {
        _datasheets = LoadDatasheets();
    }

    // 🔌 Function exposed to Semantic Kernel
    [KernelFunction]
    public string GetAttribute(
        string designation,
        string attribute)
    {
        if (string.IsNullOrWhiteSpace(designation) ||
            string.IsNullOrWhiteSpace(attribute))
        {
            return "Designation or attribute missing.";
        }

        if (!_datasheets.ContainsKey(designation))
            return $"No datasheet found for '{designation}'.";

        var sheet = _datasheets[designation];

        if (!sheet.TryGetProperty(attribute, out var value))
            return $"Attribute '{attribute}' not found for '{designation}'.";

        return $"The {attribute} of {designation} is {value}.";
    }

    // 📂 Load JSON datasheets
    private Dictionary<string, JsonElement> LoadDatasheets()
    {
        var dict = new Dictionary<string, JsonElement>();

        var files = new[]
        {
            "Datasheets/6205.json",
            "Datasheets/6025 N.json"
        };

        foreach (var file in files)
        {
            var json = File.ReadAllText(file);
            var doc = JsonDocument.Parse(json);

            var designation =
                doc.RootElement
                   .GetProperty("designation")
                   .GetString();

            dict[designation] = doc.RootElement;
        }

        return dict;
    }
}
