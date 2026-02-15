using System.ComponentModel;
using Spectre.Console.Cli;

namespace Shiron.Docs.Cli.Commands;

[Description("Builds the Shiron Docs static site. And starts a local Vite server to preview the site.")]
public sealed class CommandDev : AsyncCommand<CommandDev.Settings> {
    public class Settings : CommandSettings {
    }

    public override Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}
