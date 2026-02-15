using System.Diagnostics;
using CliWrap;
using CliWrap.Buffered;

namespace Shiron.Docs.Engine.utils;

public readonly record struct ShellResult(string StdOut, string StdErr, int ExitCode);

public static class ShellUtils {
    public static async Task<ShellResult> RunAsync(string command, string[] args, string? workingDirectory) {
        var builder = Cli.Wrap(command).WithArguments(args);
        if (workingDirectory != null) {
            builder.WithWorkingDirectory(workingDirectory);
        }
        var res = await builder.ExecuteBufferedAsync();
        return new ShellResult(res.StandardOutput, res.StandardError, res.ExitCode);
    }
}
