

using Spectre.Console.Cli;

namespace Shiron.Docs.Cli.Commands.New;

public class CommandNewSettings : CommandSettings {
    [CommandArgument(0, "<title>")]
    public string? Title { get; set; } = null;
}
