
using BenchmarkDotNet.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shiron.Docs.Engine.Extractor;

namespace Shiron.Docs.Engine.Benchmarks;

[MemoryDiagnoser]
public class ExtractorBenchmarks {
    // --- State for Demo Project ---
    private CSharpExtractor _demoExtractor = null!;

    // --- State for Standard Library ---
    private CSharpExtractor _stdLibExtractor = null!;
    private IAssemblySymbol _stdLibSymbol = null!;

    [GlobalSetup(Target = nameof(BenchmarkDemo))]
    public void SetupDemo() {
        CSharpExtractor.Init();
        _demoExtractor = new CSharpExtractor();

        var projectPath = Path.Combine(AppContext.BaseDirectory, "SrcFiles", "demo", "Shiron.DemoProject.csproj");

        if (!File.Exists(projectPath))
            throw new FileNotFoundException($"Could not find demo project at: {projectPath}");

        _demoExtractor.AddProject(projectPath);
    }

    [IterationSetup(Target = nameof(BenchmarkDemo))]
    public void ResetDemo() {
        _demoExtractor.Reset();
    }

    [IterationSetup(Target = nameof(BenchmarkStdLib))]
    public void ResetStdLib() {
        _stdLibExtractor.Reset();
    }

    [Benchmark]
    public async Task BenchmarkDemo() {
        _ = await _demoExtractor.ExtractAsync();
    }

    [GlobalSetup(Target = nameof(BenchmarkStdLib))]
    public void SetupStdLib() {
        CSharpExtractor.Init();
        _stdLibExtractor = new CSharpExtractor();

        var coreLibPath = typeof(object).Assembly.Location;
        var reference = MetadataReference.CreateFromFile(coreLibPath);

        var compilation = CSharpCompilation.Create(
            "CoreLibTest",
            references: [reference]
        ) ?? throw new Exception("Failed to create compilation during setup.");

        _stdLibSymbol = compilation.GetAssemblyOrModuleSymbol(reference) as IAssemblySymbol
            ?? throw new Exception("Failed to get assembly symbol for CoreLib during setup.");
    }

    [Benchmark]
    public async Task BenchmarkStdLib() {
        _ = await _stdLibExtractor.ExtractAssemblySymbolAsync(_stdLibSymbol, "System.Private.CoreLib.dll");
    }
}
