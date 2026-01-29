
namespace Shiron.Docs.Cli.Exceptions;

public class ProcessException(string message, string command, string stdout, string stderr, int exitCode) : Exception(message) {
    public readonly string Command = command;
    public readonly string Stdout = stdout;
    public readonly string Stderr = stderr;
    public readonly int ExitCode = exitCode;

    public override string ToString() {
        return $"{base.ToString()}\nExit Code: {ExitCode}\nStandard Output: {Stdout}\nStandard Error: {Stderr}";
    }
}
