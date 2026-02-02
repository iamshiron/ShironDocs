
using System.ComponentModel;
using System.IO.Compression;
using System.Text.Json;
using Shiron.Docs.Cli.Services;
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
        [CommandOption("-t|--template <template_file>")]
        public string TemplateFile { get; set; } = "template.zip";
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken) {
        AnsiConsole.MarkupLine($"{CLIConstants.Prefix} [bold green]Bootstrapping Shiron Docs environment...[/]");

        await _configManager.LoadConfigAsync();
        var config = _configManager.Config;

        var templateFile = Path.Combine(Directory.GetCurrentDirectory(), settings.TemplateFile);
        if (!File.Exists(templateFile)) {
            AnsiConsole.MarkupLine($"{CLIConstants.Prefix} [bold red]Error:[/] Template file '{templateFile}' does not exist.");
            return ExitCodes.UnknownError;
        }

        AnsiConsole.MarkupLine($"{CLIConstants.Prefix} [bold aqua]Extracting template files...[/]");
        using var zipFile = ZipFile.OpenRead(templateFile);
        foreach (var entry in zipFile.Entries) {
            var destinationPath = Path.Combine(config.OutputDirectory, entry.FullName);
            var isDirectory = entry.FullName.EndsWith("/") || string.IsNullOrEmpty(entry.Name);
            if (isDirectory) {
                _ = Directory.CreateDirectory(destinationPath);
                continue;
            }

            var destinationDir = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(destinationDir) && !Directory.Exists(destinationDir)) {
                _ = Directory.CreateDirectory(destinationDir);
            }

            entry.ExtractToFile(destinationPath, overwrite: true);
        }

        AnsiConsole.MarkupLine($"{CLIConstants.Prefix} [bold aqua]Setting up Vite server...[/]");
        await CLIServices.BootstrapViteServerAsync(config);

        AnsiConsole.MarkupLine($"{CLIConstants.Prefix} [bold green]Bootstrap completed successfully![/]");
        return ExitCodes.Success;
    }
}
