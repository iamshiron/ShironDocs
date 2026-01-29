
using System.ComponentModel;
using System.Text.Json;
using Shiron.Docs.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Shiron.Docs.Cli.Commands;

[Description("Bootstraps the Shiron Docs environment including the Vite project.")]
public class CommandBootstrap : AsyncCommand<CommandBootstrap.Settings> {
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions {
        WriteIndented = true,
        IndentSize = 4
    };

    public class Settings : CommandSettings {
        [CommandOption("-o|--output <OUTPUT_DIR>")]
        public string? OutputDir { get; set; } = null;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken) {
        var outputDir = settings.OutputDir == null ? Directory.GetCurrentDirectory() : Path.Join(Directory.GetCurrentDirectory(), settings.OutputDir);

        var packageJson = NpmUtils.GeneratePackageJson();
        await File.WriteAllTextAsync("package.json", JsonSerializer.Serialize(packageJson, _jsonOptions), cancellationToken);

        var packageManager = PackageManagerUtils.DetectPackageManager();
        Log.Info($"Using package manager: {packageManager.Command} - {await packageManager.GetVersionAsync()}");

        AnsiConsole.Status().Spinner(Spinner.Known.Moon).SpinnerStyle(Style.Parse("yellow")).Start("Installing NPM packages...", ctx => {
            packageManager.InstallAsync(outputDir).GetAwaiter().GetResult();
        });

        AnsiConsole.WriteLine();
        Log.Success("Bootstrap completed successfully.");

        return await Task.FromResult(0);
    }
}
