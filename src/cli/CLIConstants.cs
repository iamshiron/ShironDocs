
using Spectre.Console;

namespace Shiron.Docs.Cli;

public static class CLIConstants {
    public static readonly string Version = "1.0.0";
    public static readonly string AppName = "shirondocs";
    public static readonly string Prefix = "[DarkViolet][[Shiron Docs]][/]";
}

public static class ExitCodes {
    public static readonly int UnknownError = -1;
    public static readonly int Success = 0;
    public static readonly int ConfigNotFound = 1;
    public static readonly int ViteServerNotSetup = 2;

    public static string GetErrorMessage(int code) {
        if (code == UnknownError) {
            return "An unknown error occurred.";
        }
        if (code == ConfigNotFound) {
            return "Configuration file not found.";
        }
        if (code == ViteServerNotSetup) {
            return "Vite server is not set up.";
        }

        return "No error.";
    }
    public static void PrintHelp(int code) {
        if (code == UnknownError) {
            AnsiConsole.MarkupLine($"{CLIConstants.Prefix} [bold red]An unknown error occurred.[/]");
            AnsiConsole.MarkupLine($"{CLIConstants.Prefix} This usually indicates a bug or an error in your C# projects.");
            return;
        }
        if (code == ConfigNotFound) {
            AnsiConsole.MarkupLine($"{CLIConstants.Prefix} [bold red]Configuration file not found.[/]");
            AnsiConsole.MarkupLine($"{CLIConstants.Prefix} Run [bold]{CLIConstants.AppName} init[/] to create a new configuration.");
            return;
        }
        if (code == ViteServerNotSetup) {
            AnsiConsole.MarkupLine($"{CLIConstants.Prefix} [bold red]Vite server is not set up.[/]");
            AnsiConsole.MarkupLine($"{CLIConstants.Prefix} Run [bold]{CLIConstants.AppName} bootstrap[/] to set up the Vite server.");
            return;
        }

        AnsiConsole.MarkupLine($"{CLIConstants.Prefix} [bold red]An unspecified error occurred. This should not have happened.[/]");
    }
}
