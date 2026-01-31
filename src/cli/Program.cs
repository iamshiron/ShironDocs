
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

var jsonSerializerOptions = new JsonSerializerOptions {
    WriteIndented = true,
    IndentSize = 4
};

#pragma warning disable CS8321 // Local function is declared but never used
async Task RunDemoProject() {
    Console.WriteLine("Running demo project extraction...");

    var extractor = new CSharpExtractor();
    extractor.AddProject("./src/demo/Shiron.DemoProject.csproj");

    long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    var res = await extractor.ExtractAsync();
    long endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    Console.WriteLine($"Extraction completed in {endTime - startTime} ms.");

    if (File.Exists("output.json")) {
        File.Delete("output.json");
    }
    var fileStream = File.Create("output.json");
    await JsonSerializer.SerializeAsync(fileStream, res, jsonSerializerOptions);
    fileStream.Close();
}

async Task RunStandardLibrary() {
    Console.WriteLine("Running Standard Library Extraction...");

    var coreLibPath = typeof(object).Assembly.Location;
    Console.WriteLine($"Core Library Path: {coreLibPath}");

    var reference = MetadataReference.CreateFromFile(coreLibPath);
    var compilation = CSharpCompilation.Create(
        "CoreLibTest",
        references: [reference]
    );

    var assemblySymbol = compilation.GetAssemblyOrModuleSymbol(reference) as IAssemblySymbol;
    if (assemblySymbol == null) {
        Console.WriteLine("Failed to get assembly symbol.");
        return;
    }

    Console.WriteLine($"Loaded Assembly: {assemblySymbol.Name}v{assemblySymbol.Identity.Version}");

    var extractor = new CSharpExtractor();
    long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    var res = await extractor.ExtractAssemblySymbolAsync(assemblySymbol, "System.Private.CoreLib.dll");
    long endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    Console.WriteLine($"Extraction completed in {endTime - startTime} ms.");

    if (File.Exists("System.Private.CoreLib.json")) {
        File.Delete("System.Private.CoreLib.json");
    }
    var fileStream = File.Create("System.Private.CoreLib.json");
    await JsonSerializer.SerializeAsync(fileStream, res, jsonSerializerOptions);
    fileStream.Close();
}
#pragma warning restore CS8321 // Local function is declared but never used

CSharpExtractor.Init();
await RunDemoProject();
// await RunStandardLibrary();

foreach (var kvp in XMLDocExtractor.MissedTokens) {
    Console.WriteLine($"Missed Token: {kvp.Key}");
    foreach (var instance in kvp.Value) {
        Console.WriteLine(instance);
    }
}
