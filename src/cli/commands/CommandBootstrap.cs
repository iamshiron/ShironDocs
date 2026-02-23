using System.ComponentModel;
using Shiron.Docs.Cli.Services;
using Shiron.Docs.Engine;
using Shiron.Docs.Engine.PM;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Shiron.Docs.Cli.Commands;

[Description("Bootstraps the Shiron Docs environment including the Vite project.")]
public sealed class CommandBootstrap(IConfigManager configManager) : AsyncCommand<CommandBootstrap.Settings> {
    private readonly IConfigManager _configManager = configManager;

    public class Settings : CommandSettings {
    }

    public async override Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken) {
        AnsiConsole.MarkupLine($"{CLIConstants.Prefix} Bootstrapping Shiron Docs environment...");

        await _configManager.LoadConfigAsync();
        var config = _configManager.Config;
        var pm = await PackageManagerUtils.DetectPackageManager();

        AnsiConsole.MarkupLine($"{CLIConstants.Prefix} [bold aqua]Setting up Astro...[/]");
        if (!Directory.Exists(config.OutputDirectory)) {
            Directory.CreateDirectory(config.OutputDirectory);
        }

        var astro = new AstroWebService(config.OutputDirectory);
        await astro.BootstrapAsync(config, pm);

        AnsiConsole.MarkupLine($"{CLIConstants.Prefix} [bold aqua]Installing packages...[/]");
        await pm.InstallAsync(config.OutputDirectory);

        AnsiConsole.MarkupLine($"{CLIConstants.Prefix} [bold aqua]Performing build...[/]");
        await astro.BuildAsync(config, pm);

        AnsiConsole.MarkupLine($"{CLIConstants.Prefix} [bold green]Bootstrap completed successfully![/]");
        return ExitCodes.Success;
    }
}
