
using System.ComponentModel;
using System.Text.Json;
using Shiron.Docs.Cli.Utils;
using Shiron.Docs.Engine;
using Shiron.Docs.Engine.PM;
using Shiron.Docs.Engine.Vite;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Shiron.Docs.Cli.Commands;

[Description("Bootstraps the Shiron Docs environment including the Vite project.")]
public sealed class CommandBootstrap(IConfigManager configManager) : AsyncCommand<CommandBootstrap.Settings> {
    private readonly IConfigManager _configManager = configManager;

    public class Settings : CommandSettings {
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken) {
        AnsiConsole.MarkupLine("[bold green]Bootstrapping Shiron Docs environment...[/]");

        await _configManager.LoadConfigAsync("shirondocs.json");
        System.Console.WriteLine($"Config: {_configManager.Config.AppName}");

        // Needs to be auto-detected or user-specified in the future
        var packageManager = new PNPMPackageManager();
        var viteBootstrap = new ViteBootstrap(new ViteBootstrapOptions() {
            OutputDir = "_site"
        });

        await viteBootstrap.BootstrapAsync(packageManager);

        AnsiConsole.MarkupLine("[bold green]Bootstrap completed successfully![/]");
        return 0;
    }
}
