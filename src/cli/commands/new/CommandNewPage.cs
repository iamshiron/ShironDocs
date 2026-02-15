using System.ComponentModel;
using Spectre.Console.Cli;

namespace Shiron.Docs.Cli.Commands.New;

[Description("Bootstraps a new documentation page in your documentation site.")]
public sealed class CommandNewPage : AsyncCommand<CommandNewPage.Settings> {
    public class Settings : CommandSettings {
    }

    public override Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}
