
using System.Reflection;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Shiron.Docs.Engine.Model;

namespace Shiron.Docs.Engine.Extractor;

public class CSharpExtractor {
    private string _root;
    private readonly MSBuildWorkspace _workspace;
    private List<Project> _projects = [];

    private Dictionary<string, Compilation> _compilationCache = [];
    private Dictionary<ISymbol, string> _docIDCache = [];

    public static void Init() {
        MSBuildLocator.RegisterDefaults();
    }

    public CSharpExtractor() {
        _workspace = MSBuildWorkspace.Create();
        _root = Directory.GetCurrentDirectory();
    }

    public async Task TaskLoadSolutionAsync(string path) {
        var sln = await _workspace.OpenSolutionAsync(path);
        _projects = [.. sln.Projects];
        _root = sln.FilePath!;
    }

    public void AddProject(string path) {
        if (!path.EndsWith(".csproj")) {
            throw new ArgumentException("Path must be a .csproj file", nameof(path));
        }
        if (!File.Exists(path)) {
            throw new FileNotFoundException("Project file not found", path);
        }

        var project = _workspace.OpenProjectAsync(path).Result;
        _projects.Add(project);
    }

    private async Task<Compilation> GetCompilationAsync(Project project) {
        var projectID = project.Id.ToString();
        if (_compilationCache.TryGetValue(projectID, out var compilation)) {
            return compilation;
        }

        compilation = await project.GetCompilationAsync() ?? throw new Exception("Failed to get compilation");
        _compilationCache[projectID] = compilation;
        return compilation;
    }
    private IEnumerable<INamedTypeSymbol> GetNamedTypes(IAssemblySymbol assembly, INamespaceSymbol @namespace) {
        foreach (var type in @namespace.GetTypeMembers()) {
            if (!SymbolEqualityComparer.Default.Equals(type.ContainingAssembly, assembly)) {
                continue;
            }
            yield return type;
        }
        foreach (var subNs in @namespace.GetNamespaceMembers()) {
            foreach (var type in GetNamedTypes(assembly, subNs)) {
                if (!SymbolEqualityComparer.Default.Equals(type.ContainingAssembly, assembly)) {
                    continue;
                }
                yield return type;
            }
        }
    }

    public string GetDocID(ISymbol symbol) {
        if (_docIDCache.TryGetValue(symbol, out var docID)) {
            return docID;
        }
        docID = symbol.GetDocumentationCommentId();

        if (string.IsNullOrEmpty(docID)) {
            docID = $"U:{symbol.ToDisplayString()}";
        }

        _docIDCache[symbol] = docID;
        return docID;
    }

    public string PathRelativeTo(string path) {
        return Path.GetRelativePath(_root, path);
    }

    public async Task<List<AssemblyData>> ExtractAsync() {
        Console.WriteLine($"Root Path: {_root}");

        var res = new List<AssemblyData>();
        foreach (var project in _projects) {
            res.Add(await ExtractAssemblyAsync(project));
        }
        return res;
    }

    public async Task<AssemblyData> ExtractAssemblyAsync(Project project) {
        var compilation = await GetCompilationAsync(project);
        var assembly = compilation.Assembly;

        var assemblyData = new AssemblyData(
            Name: assembly.Name,
            Version: assembly.Identity.Version.ToString(),
            CSProjFile: PathRelativeTo(project.FilePath!),
            Symbols: new List<DocumentationSymbol>(),
            MethodMetadata: new List<MethodData>()
        );
        return assemblyData;
    }
}
