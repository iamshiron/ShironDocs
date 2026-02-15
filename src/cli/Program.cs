using System.Globalization;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Extensions.DependencyInjection;
using Shiron.Docs.Cli;
using Shiron.Docs.Cli.Commands;
using Shiron.Docs.Cli.Commands.New;
using Shiron.Docs.Cli.DI;
using Shiron.Docs.Engine;
using Spectre.Console;
using Spectre.Console.Cli;

#if DEBUG
if (!Directory.Exists("run")) {
    Directory.CreateDirectory("run");
}
Directory.SetCurrentDirectory("run/");

Console.WriteLine($"Set Current Directory to: {Directory.GetCurrentDirectory()}");
#endif

var services = new ServiceCollection();
services.AddSingleton<IConfigManager, ConfigManager>();

var registrar = new TypeRegistrar(services);
var app = new CommandApp(registrar);
app.Configure(c => {
    _ = c.PropagateExceptions();

    _ = c.SetApplicationName(CLIConstants.AppName);
    _ = c.SetApplicationVersion(CLIConstants.Version);

    _ = c.SetApplicationCulture(CultureInfo.CurrentCulture);

    _ = c.AddCommand<CommandBootstrap>("bootstrap");
    _ = c.AddCommand<CommandBuild>("build");
    _ = c.AddCommand<CommandDev>("dev");
    _ = c.AddCommand<CommandStart>("start");
    _ = c.AddCommand<CommandInit>("init");
    _ = c.AddCommand<CommandBuildCore>("debug-build-core");

    _ = c.AddBranch("new", b => {
        b.SetDescription("Create new content for the documentation site.");

        _ = b.AddCommand<CommandNewPage>("page");
    });
});

try {
    var res = await app.RunAsync(args);
    if (res != 0) {
        Environment.ExitCode = res;

        AnsiConsole.MarkupLine($"{CLIConstants.Prefix} [bold red]Error:[/] Command exited with code {res} ({ExitCodes.GetErrorMessage(res)}).");
        ExitCodes.PrintHelp(res);
    }
} catch (NotImplementedException) {
    AnsiConsole.MarkupLine($"{CLIConstants.Prefix} [bold red]Error:[/] Command not implemented.");
} catch (Exception e) {
    AnsiConsole.WriteException(e);
}
