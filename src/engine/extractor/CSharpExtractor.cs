
using System.Collections.Concurrent;
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
            Namespaces: new ConcurrentDictionary<string, NamespaceSymbol>(),
            Types: new ConcurrentDictionary<string, TypeSymbol>(),
            Methods: new ConcurrentDictionary<string, MethodSymbol>(),
            Properties: new ConcurrentDictionary<string, PropertySymbol>(),
            Fields: new ConcurrentDictionary<string, FieldSymbol>(),
            Enums: new ConcurrentDictionary<string, EnumSymbol>(),
            Errors: new ConcurrentDictionary<string, ErrorSymbol>()
        );

        var types = GetNamedTypes(assembly, assembly.GlobalNamespace);
        foreach (var type in types) {
            await ExtractSymbolAsync(type, assemblyData);
        }

        return assemblyData;
    }

    public async Task ExtractSymbolAsync(ISymbol typeSymbol, AssemblyData context, ISymbolContainer? parent = null) {
        var id = GetDocID(typeSymbol);

        var addChild = false;
        switch (typeSymbol) {
            case ITypeSymbol s:
                addChild = await ExtractTypeAsync(id, s, context);
                break;

            case IMethodSymbol s:
                addChild = ExtractMethod(id, s, context);
                break;

            default:
                break;
        }

        if (addChild && parent != null) {
            parent.ChildIDs.Add(id);
        }
    }

    public async Task<bool> ExtractTypeAsync(string id, ITypeSymbol typeSymbol, AssemblyData context) {
        var symbol = new TypeSymbol(
            Name: typeSymbol.Name
        );
        context.Types[id] = symbol;

        foreach (var member in typeSymbol.GetMembers()) {
            await ExtractSymbolAsync(member, context, symbol);
        }

        return true;
    }

    public bool ExtractMethod(string id, IMethodSymbol methodSymbol, AssemblyData context) {
        // Only extract ordinary methods and constructors
        if (methodSymbol.MethodKind != MethodKind.Ordinary && methodSymbol.MethodKind != MethodKind.Constructor) {
            return false;
        }

        var parameters = new ParameterItem[methodSymbol.Parameters.Length];

        for (int i = 0; i < methodSymbol.Parameters.Length; i++) {
            var param = methodSymbol.Parameters[i];
            parameters[i] = new ParameterItem(
                Name: param.Name,
                TypeID: GetDocID(param.Type),
                TypeName: param.Type.ToDisplayString()
            );
        }

        var symbol = new MethodSymbol(
            Name: methodSymbol.Name,
            ReturnTypeID: GetDocID(methodSymbol.ReturnType),
            ReturnTypeName: methodSymbol.ReturnType.ToDisplayString(),
            Parameters: parameters
        );
        context.Methods[id] = symbol;
        return true;
    }
}
