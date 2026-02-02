using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shiron.Docs.Engine;

public class Config {
    [JsonPropertyName("includes")]
    public string[] Includes { get; set; } = [];
    [JsonPropertyName("excludes")]
    public string[] Excludes { get; set; } = [];

    [JsonPropertyName("outputDirectory")]
    public string OutputDirectory { get; set; } = "_site";

    [JsonPropertyName("appName")]
    public string AppName { get; set; } = "Shiron Docs";
    [JsonPropertyName("appVersion")]
    public string AppVersion { get; set; } = "1.0.0";
    [JsonPropertyName("appFooter")]
    public string AppFooter { get; set; } = "Powered by Shiron Docs";
}

public interface IConfigManager {
    Config Config { get; }
    Task LoadConfigAsync(string path);
}

public class ConfigManager : IConfigManager {
    public Config Config { get; private set; } = new();

    public async Task LoadConfigAsync(string path) {
        if (!File.Exists(path)) {
            return;
        }

        var json = await File.ReadAllTextAsync(path);
        var config = JsonSerializer.Deserialize<Config>(json);

        if (config != null) {
            Config = config;
        }
    }
}
