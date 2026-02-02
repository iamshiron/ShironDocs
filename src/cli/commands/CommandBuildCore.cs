
using System.ComponentModel;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shiron.Docs.Engine;
using Shiron.Docs.Engine.Extractor;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Shiron.Docs.Cli.Commands;

[Description("Builds the .NET core documentation site.")]
public sealed class CommandBuildCore(IConfigManager configManager) : AsyncCommand<CommandBuildCore.Settings> {
    private readonly IConfigManager _configManager = configManager;

    public class Settings : CommandSettings {
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken) {
        var confirm = AnsiConsole.Prompt(
            new ConfirmationPrompt($"{CLIConstants.Prefix} This will build the .NET core documentation site. Do you want to continue? This is meant for debugging purposes only.")
                .ShowChoices()
        );

        if (!confirm) {
            AnsiConsole.MarkupLine($"{CLIConstants.Prefix} [yellow]Operation cancelled by user.[/]");
            return ExitCodes.Success;
        }

        AnsiConsole.MarkupLine($"{CLIConstants.Prefix} [bold green]Building .NET core documentation site...[/]");
        await _configManager.LoadConfigAsync();
        var config = _configManager.Config;

        CSharpExtractor.Init();
        var extractor = new CSharpExtractor();

        var coreLibPath = typeof(object).Assembly.Location;
        AnsiConsole.MarkupLine($"{CLIConstants.Prefix} [bold green]Using core library at: [/]{coreLibPath}");

        var reference = MetadataReference.CreateFromFile(coreLibPath);
        var compilation = CSharpCompilation.Create(
            "CoreLibTest",
            references: [reference]
        );
        var assemblySymbol = compilation.GetAssemblyOrModuleSymbol(reference) as IAssemblySymbol;
        if (assemblySymbol == null) {
            AnsiConsole.MarkupLine($"{CLIConstants.Prefix} [red]Failed to get assembly symbol.[/]");
            return ExitCodes.UnknownError;
        }

        var assemblyData = await extractor.ExtractAssemblySymbolAsync(assemblySymbol, "System.Private.CoreLib.dll");
        await JsonSerializer.SerializeAsync(File.OpenWrite("output.json"), assemblyData, new JsonSerializerOptions {
            WriteIndented = true,
            IndentSize = 4
        });

        return ExitCodes.Success;
    }
}
