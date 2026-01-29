
using Spectre.Console;

namespace Shiron.Docs.Cli.Utils;

public static class Log {
    public static void Print(string message) {
        try {
            AnsiConsole.MarkupLine(message);
        } catch {
            AnsiConsole.WriteLine($"Unable to format log message.\nOriginal message: {message}");
        }
    }

    public static void Debug(string message) {
        Print($"[grey][[DEBUG]][/] {message}");
    }

    public static void Info(string message) {
        Print($"[white][[INFO]][/] {message}");
    }

    public static void Warn(string message) {
        Print($"[yellow][[WARN]][/] {message}");
    }

    public static void Error(string message) {
        Print($"[red][[ERROR]][/] {message}");
    }

    public static void Success(string message) {
        Print($"[green][[SUCCESS]][/] {message}");
    }
}
