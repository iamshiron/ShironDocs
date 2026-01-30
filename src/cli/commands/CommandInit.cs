
using System.ComponentModel;
using Spectre.Console.Cli;

namespace Shiron.Docs.Cli.Commands;

[Description("Initializes a new Shiron Docs documentation site. Supports interactive mode.")]
public sealed class CommandInit : AsyncCommand<CommandInit.Settings> {
    public class Settings : CommandSettings {
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}
