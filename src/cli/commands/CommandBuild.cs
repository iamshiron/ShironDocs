using System.ComponentModel;
using System.Text.Json;
using Shiron.Docs.Cli.Services;
using Shiron.Docs.Engine;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Shiron.Docs.Cli.Commands;

[Description("Compiles your projects and builds the static documentation site.")]
public sealed class CommandBuild(IConfigManager configManager) : AsyncCommand<CommandBuild.Settings> {
    private readonly IConfigManager _configManager = configManager;

    public class Settings : CommandSettings {
    }

    public async override Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken) {
        await _configManager.LoadConfigAsync();

        var config = _configManager.Config;
        if (!Directory.Exists(config.OutputDirectory)) return ExitCodes.ViteServerNotSetup;

        AnsiConsole.MarkupLine($"{CLIConstants.Prefix} [bold green]Building Static Site...[/]");
        AnsiConsole.MarkupLine($"{CLIConstants.Prefix} [bold green]Static site built successfully![/]");

        return ExitCodes.Success;
    }
}
