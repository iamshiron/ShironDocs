
using System.ComponentModel;
using Spectre.Console.Cli;

namespace Shiron.Docs.Cli.Commands;

[Description("Compiles your projects and builds the static documentation site.")]
public sealed class CommandBuild : AsyncCommand<CommandBuild.Settings> {
    public class Settings : CommandSettings {
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}
