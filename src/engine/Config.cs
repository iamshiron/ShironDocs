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
    Task LoadConfigAsync();
    Task SaveConfigAsync(Config config);
}

public class ConfigManager : IConfigManager {
    public static string ConfigFileName { get; } = "shirondocs.json";

    public Config Config { get; private set; } = new();

    public async Task LoadConfigAsync() {
        if (!File.Exists(ConfigFileName)) {
            return;
        }

        var json = await File.ReadAllTextAsync(ConfigFileName);
        var config = JsonSerializer.Deserialize<Config>(json);

        if (config != null) {
            Config = config;
        }
    }

    public async Task SaveConfigAsync(Config config) {
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(ConfigFileName, json);
    }
}
