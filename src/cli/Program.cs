
using System.Globalization;
using System.Text.Json;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using Shiron.Docs.Cli.Commands;
using Shiron.Docs.Cli.Commands.New;
using Shiron.Docs.Engine.Extractor;
using Shiron.Docs.Engine.Model;
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
    _ = c.AddCommand<CommandBuild>("build");
    _ = c.AddCommand<CommandDev>("dev");
    _ = c.AddCommand<CommandStart>("start");
    _ = c.AddCommand<CommandInit>("init");

    _ = c.AddBranch("new", b => {
        b.SetDescription("Create new content for the documentation site.");

        _ = b.AddCommand<CommandNewPage>("page");
    });
});

await app.RunAsync(args);

CSharpExtractor.Init();
var extractor = new CSharpExtractor();
extractor.AddProject("src/demo/Shiron.DemoProject.csproj");

var res = await extractor.ExtractAsync();
var json = JsonSerializer.Serialize(res, new JsonSerializerOptions {
    WriteIndented = true
});

File.WriteAllText("output.json", json);
Console.WriteLine("Extraction complete. Output written to output.json.");
