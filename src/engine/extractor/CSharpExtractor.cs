
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

    private string GetDisplayName(ISymbol symbol) {
        return symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
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
            Errors: new ConcurrentDictionary<string, ErrorSymbol>()
        );

        var types = GetNamedTypes(assembly, assembly.GlobalNamespace);

        BuildNamespaceTree(types, assemblyData);
        foreach (var type in types) {
            var docID = GetDocID(type);
            await ExtractSymbolAsync(docID, type, assemblyData);
        }

        return assemblyData;
    }

    private string GetNamespaceID(INamespaceSymbol symbol) => $"N:{symbol.ToDisplayString()}";

    private void BuildNamespaceTree(IEnumerable<ITypeSymbol> types, AssemblyData context) {
        // 1. Get unique namespaces only (Performance Win)
        // SymbolEqualityComparer is crucial for Roslyn keys
        var uniqueNamespaces = types
            .Select(t => t.ContainingNamespace)
            .Where(n => n != null && !n.IsGlobalNamespace)
            .Distinct(SymbolEqualityComparer.Default)
            .Cast<INamespaceSymbol>();

        // 2. Build the skeleton for these few namespaces
        foreach (var ns in uniqueNamespaces) {
            EnsureNamespaceRecursive(ns, context);
        }
    }

    private void EnsureNamespaceRecursive(INamespaceSymbol ns, AssemblyData context) {
        var id = GetDocID(ns);
        if (context.Namespaces.ContainsKey(id)) return;
        var nsSymbol = new NamespaceSymbol(
            Name: ns.Name
        );

        if (context.Namespaces.TryAdd(id, nsSymbol)) {
            if (ns.ContainingNamespace != null && !ns.ContainingNamespace.IsGlobalNamespace) {
                var parentId = GetDocID(ns.ContainingNamespace);
                EnsureNamespaceRecursive(ns.ContainingNamespace, context);

                if (context.Namespaces.TryGetValue(parentId, out var parentNs)) {
                    parentNs.ChildIDs.Add(id);
                }
            }
        }
    }

    public async Task ExtractSymbolAsync(string id, ISymbol typeSymbol, AssemblyData context, ISymbolContainer? parent = null) {
        var addChild = false;
        switch (typeSymbol) {
            case ITypeSymbol s:
                addChild = await ExtractTypeAsync(id, s, context);
                break;

            case IMethodSymbol s:
                addChild = ExtractMethod(id, s, context);
                break;

            case IPropertySymbol s:
                addChild = ExtractProperty(id, s, context);
                break;

            case IFieldSymbol s:
                addChild = ExtractField(id, s, context);
                break;

            default:
                break;
        }

        if (addChild && parent != null) {
            parent.ChildIDs.Add(id);
        }
    }
    public async Task<bool> ExtractTypeAsync(string id, ITypeSymbol typeSymbol, AssemblyData context) {
        var namespaceDocID = GetDocID(typeSymbol.ContainingNamespace);
        if (context.Namespaces.TryGetValue(namespaceDocID, out var nsSymbol)) {
            nsSymbol.ChildIDs.Add(id);
        }

        if (typeSymbol.TypeKind == TypeKind.Enum) {
            return ExtractEnum(id, (INamedTypeSymbol) typeSymbol, context);
        }

        var symbol = new TypeSymbol(
            Name: typeSymbol.Name
        );
        context.Types[id] = symbol;

        foreach (var member in typeSymbol.GetMembers()) {
            await ExtractSymbolAsync(GetDocID(member), member, context, symbol);
        }

        return true;
    }

    public bool ExtractMethod(string id, IMethodSymbol methodSymbol, AssemblyData context) {
        if (methodSymbol.MethodKind != MethodKind.Ordinary && methodSymbol.MethodKind != MethodKind.Constructor) {
            return false;
        }

        var parameters = new ParameterItem[methodSymbol.Parameters.Length];

        for (int i = 0; i < methodSymbol.Parameters.Length; i++) {
            var param = methodSymbol.Parameters[i];
            parameters[i] = new ParameterItem(
                Name: param.Name,
                TypeID: GetDocID(param.Type),
                TypeName: GetDisplayName(param.Type)
            );
        }

        var symbol = new MethodSymbol(
            Name: methodSymbol.Name,
            ReturnTypeID: GetDocID(methodSymbol.ReturnType),
            ReturnTypeName: GetDisplayName(methodSymbol.ReturnType),
            Parameters: parameters
        );
        context.Methods[id] = symbol;
        return true;
    }

    public bool ExtractProperty(string id, IPropertySymbol propertySymbol, AssemblyData context) {
        var symbol = new PropertySymbol(
            Name: propertySymbol.Name,
            TypeID: GetDocID(propertySymbol.Type),
            TypeName: GetDisplayName(propertySymbol.Type)
        );
        context.Properties[id] = symbol;
        return true;
    }

    public bool ExtractField(string id, IFieldSymbol fieldSymbol, AssemblyData context) {
        // Skip compiler-generated backing fields
        if (fieldSymbol.IsImplicitlyDeclared) {
            return false;
        }

        var symbol = new FieldSymbol(
            Name: fieldSymbol.Name,
            TypeID: GetDocID(fieldSymbol.Type),
            TypeName: GetDisplayName(fieldSymbol.Type)
        );
        context.Fields[id] = symbol;
        return true;
    }

    public bool ExtractEnum(string id, INamedTypeSymbol enumSymbol, AssemblyData context) {
        if (enumSymbol.EnumUnderlyingType == null) {
            throw new Exception($"Enum underlying type is null. Is {enumSymbol.Name} really an enum?");
        }

        var members = enumSymbol.GetMembers();
        var options = new List<EnumItem>(members.Length);
        foreach (var member in members) {
            if (member.Kind == SymbolKind.Field && member is IFieldSymbol fs && fs.ConstantValue != null) {
                var docID = GetDocID(fs);

                options.Add(new EnumItem(
                    Name: fs.Name,
                    Value: fs.ConstantValue!.ToString()!,
                    ID: docID
                ));
                context.Fields[docID] = new FieldSymbol(
                    Name: fs.Name,
                    TypeID: GetDocID(fs.Type),
                    TypeName: GetDisplayName(fs.Type)
                );
            }
        }

        var symbol = new EnumSymbol(
            Name: enumSymbol.Name,
            UnderlyingTypeID: GetDocID(enumSymbol.EnumUnderlyingType),
            UnderlyingTypeName: GetDisplayName(enumSymbol.EnumUnderlyingType),
            Options: [.. options]
        );
        context.Types[id] = symbol;
        return true;
    }
}
