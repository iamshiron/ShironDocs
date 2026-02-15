using System.IO.Compression;
using Microsoft.Extensions.FileSystemGlobbing;
using Shiron.Docs.Engine;
using Shiron.Docs.Engine.Builder;
using Shiron.Docs.Engine.Extractor;
using Shiron.Docs.Engine.Model;
using Shiron.Docs.Engine.PM;

namespace Shiron.Docs.Cli.Services;

public static class CLIServices {
    public static async Task InitViteServerFilesAsync(Config config, string templateFile) {
        var zipFile = await ZipFile.OpenReadAsync(templateFile);
        await zipFile.ExtractToDirectoryAsync(config.OutputDirectory);
    }
    public static async Task InstallDependenciesAsync(Config config, IPackageManager pm) {
        await pm.InstallAsync(config.OutputDirectory);
    }

    public static string[] GetProjectFiles(Config config) {
        var fileMatcher = new Matcher();
        fileMatcher.AddIncludePatterns(config.Includes);
        fileMatcher.AddExcludePatterns(config.Excludes);

        var projectFiles = fileMatcher.GetResultsInFullPath(Directory.GetCurrentDirectory());
        return [.. projectFiles];
    }
    public static async Task<List<AssemblyData>> GenerateAssemblyInfo(string[] projectFiles) {
        CSharpExtractor.Init();
        var extractor = new CSharpExtractor();

        foreach (var projectFile in projectFiles) extractor.AddProject(projectFile);

        return await extractor.ExtractAsync();
    }
    public static async Task GenerateDocumenationSitesAsync(Config config, AssemblyData[] assemblies) {
        var builder = new TSXBuilder(Path.Combine(config.OutputDirectory, "src/pages/api"));
        foreach (var assembly in assemblies) await builder.BuildAssemblyAsync(assembly);
    }

    public static Task BuildSiteAsync() {
        throw new NotImplementedException();
    }
}
