
namespace Shiron.Docs.Engine.Model;

public record ParameterItem(
    string Name,
    string TypeID
);

public record NamespaceSymbol(
);
public record TypeSymbol(
    string Name,
    List<string> MethodIDs,
    List<string> PropertyIDs,
    List<string> FieldIDs
);
public record MemberSymbol(
);
public record PropertySymbol(
);
public record FieldSymbol(
);
public record EnumSymbol(
);
public record MethodSymbol(
    string Name,
    string ReturnTypeID,
    List<ParameterItem> Parameters
);
public record ErrorSymbol(
);

public record AssemblyData(
    string Name,
    string Version,
    string CSProjFile,
    Dictionary<string, NamespaceSymbol> Namespaces,
    Dictionary<string, TypeSymbol> Types,
    Dictionary<string, MethodSymbol> Methods,
    Dictionary<string, PropertySymbol> Properties,
    Dictionary<string, FieldSymbol> Fields,
    Dictionary<string, EnumSymbol> Enums,
    Dictionary<string, ErrorSymbol> Errors
);
