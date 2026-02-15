using System.ComponentModel;
using System.Text.Json;
using Shiron.Docs.Cli.Services;
using Shiron.Docs.Cli.Utils;
using Shiron.Docs.Engine;
using Shiron.Docs.Engine.PM;
using Shiron.Docs.Engine.Vite;
using Spectre.Console;
using Spectre.Console.Cli;
using IPackageManager = Shiron.Docs.Engine.PM.IPackageManager;

namespace Shiron.Docs.Cli.Commands;

[Description("Bootstraps the Shiron Docs environment including the Vite project.")]
public sealed class CommandBootstrap(IConfigManager configManager) : AsyncCommand<CommandBootstrap.Settings> {
    private readonly IConfigManager _configManager = configManager;

    public class Settings : CommandSettings {
        [CommandOption("-t|--template <template_file>")]
        public string? TemplateFile { get; set; } = null;
    }

    public async override Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken) {
        AnsiConsole.MarkupLine("[bold green]Bootstrapping Shiron Docs environment...[/]");

        if (settings.TemplateFile == null) throw new ArgumentNullException(nameof(settings.TemplateFile), "As of now, template file must be specified.");

        await _configManager.LoadConfigAsync();
        var config = _configManager.Config;
        var pm = await PackageManagerUtils.DetectPackageManager();

        var templateFile = Path.Combine(Directory.GetCurrentDirectory(), settings.TemplateFile);
        if (!File.Exists(templateFile)) {
            AnsiConsole.MarkupLine($"{CLIConstants.Prefix} [bold red]Error:[/] Template file '{templateFile}' does not exist.");
            return ExitCodes.UnknownError;
        }

        AnsiConsole.MarkupLine($"{CLIConstants.Prefix} [bold aqua]Extracting template files...[/]");
        await CLIServices.InitViteServerFilesAsync(config, templateFile);

        AnsiConsole.MarkupLine($"{CLIConstants.Prefix} [bold aqua]Setting up Vite server...[/]");
        await CLIServices.InstallDependenciesAsync(config, pm);

        AnsiConsole.MarkupLine("[bold green]Bootstrap completed successfully![/]");
        return ExitCodes.Success;
    }
}
