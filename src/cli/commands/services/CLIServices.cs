
using Microsoft.Extensions.FileSystemGlobbing;
using Shiron.Docs.Engine;
using Shiron.Docs.Engine.Extractor;
using Shiron.Docs.Engine.Model;
using Shiron.Docs.Engine.PM;
using Shiron.Docs.Engine.Vite;

namespace Shiron.Docs.Cli.Services;

public static class CLIServices {
    public static async Task BootstrapViteServerAsync(Config config) {
        var packageManager = new PNPMPackageManager();
        var viteBootstrap = new ViteBootstrap(new ViteBootstrapOptions() {
            OutputDir = config.OutputDirectory
        });

        await viteBootstrap.BootstrapAsync(packageManager);
    }
    public static async Task<string[]> GetProjectFilesAsync(Config config) {
        var fileMatcher = new Matcher();
        fileMatcher.AddIncludePatterns(config.Includes);
        fileMatcher.AddExcludePatterns(config.Excludes);

        var projectFiles = fileMatcher.GetResultsInFullPath(Directory.GetCurrentDirectory());
        return [.. projectFiles];
    }
    public static async Task<List<AssemblyData>> GenerateAssemblyInfo(string[] projectFiles) {
        CSharpExtractor.Init();
        var extractor = new CSharpExtractor();

        foreach (var projectFile in projectFiles) {
            extractor.AddProject(projectFile);
        }

        return await extractor.ExtractAsync();
    }
    public static async Task BuildSiteAsync() { }
}
