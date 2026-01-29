
using System.Globalization;
using Shiron.Docs.Cli.Commands;
using Spectre.Console.Cli;

#if DEBUG
if (!Directory.Exists("run")) {
    Directory.CreateDirectory("run");
}
Directory.SetCurrentDirectory("run/");

Console.WriteLine($"Set Current Directory to: {Directory.GetCurrentDirectory()}");
#endif

var app = new CommandApp();
app.Configure(c => {
    _ = c.SetApplicationName("Shiron Docs");
    _ = c.SetApplicationVersion("0.0.0");
    _ = c.SetApplicationCulture(CultureInfo.CurrentCulture);

    _ = c.AddCommand<CommandBootstrap>("bootstrap");
});

await app.RunAsync(args);
