
using System.Diagnostics;
using Shiron.Docs.Cli.Exceptions;

namespace Shiron.Docs.Cli.Utils;

public record ProcessExitInfo(string Command, string StdOut, string StdErr, int ExitCode);

public static class ProcessUtils {
    public static async Task<ProcessExitInfo> RunProcessASync(string command, string[] arguments, string? workingDirectory = null) {
        workingDirectory ??= Directory.GetCurrentDirectory();

        Log.Debug($"Running process: {command} {string.Join(' ', arguments)}, in directory: {workingDirectory}");

        var startInfo = new ProcessStartInfo {
            FileName = command,
            Arguments = string.Join(' ', arguments),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDirectory
        };

        using var process = new Process { StartInfo = startInfo };
        var res = process.Start();
        await process.WaitForExitAsync();

        var exitCode = process.ExitCode;
        if (exitCode != 0) {
            throw new ProcessException(
                "Process failed to execute.", $"{command} {string.Join(' ', arguments)}",
                await process.StandardOutput.ReadToEndAsync(),
                await process.StandardError.ReadToEndAsync(), exitCode
            );
        }

        return new ProcessExitInfo(
            command,
            await process.StandardOutput.ReadToEndAsync(),
            await process.StandardError.ReadToEndAsync(),
            exitCode
        );
    }
}
