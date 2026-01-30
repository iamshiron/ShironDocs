
using System.ComponentModel;
using Spectre.Console.Cli;

namespace Shiron.Docs.Cli.Commands;

[Description("Start the production ready documentation site on a simple HTTP server.")]
public sealed class CommandStart : AsyncCommand<CommandStart.Settings> {
    public class Settings : CommandSettings {
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}
