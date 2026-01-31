
using System.Collections.Concurrent;

namespace Shiron.Docs.Engine.Model;

public record ParameterItem(
    string Name,
    string TypeID
);

public interface ISymbolContainer {
    ConcurrentBag<string> ChildIDs { get; }
}

public record NamespaceSymbol(
);
public record TypeSymbol(
    string Name
) : ISymbolContainer {
    public ConcurrentBag<string> ChildIDs { get; } = [];
}

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
    IDictionary<string, NamespaceSymbol> Namespaces,
    IDictionary<string, TypeSymbol> Types,
    IDictionary<string, MethodSymbol> Methods,
    IDictionary<string, PropertySymbol> Properties,
    IDictionary<string, FieldSymbol> Fields,
    IDictionary<string, EnumSymbol> Enums,
    IDictionary<string, ErrorSymbol> Errors
);
