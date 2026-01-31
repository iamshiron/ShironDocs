
using System.Collections.Concurrent;
using System.Text.Json.Serialization;

namespace Shiron.Docs.Engine.Model;

public record ParameterItem(
    string Name,
    string TypeID,
    string TypeName
);

public record EnumItem(
    string Name,
    string Value,
    string ID
);

public interface ISymbolContainer {
    ConcurrentBag<string> ChildIDs { get; }
}

public record NamespaceSymbol(
);

[JsonDerivedType(typeof(TypeSymbol), "base")]
[JsonDerivedType(typeof(EnumSymbol), "enum")]
public record TypeSymbol(
    string Name
) : ISymbolContainer {
    public ConcurrentBag<string> ChildIDs { get; } = [];
}

public record EnumSymbol(
    string Name,
    string UnderlyingTypeID,
    string UnderlyingTypeName,
    EnumItem[] Options
) : TypeSymbol(Name);

public record MemberSymbol(
);
public record PropertySymbol(
    string Name,
    string TypeID,
    string TypeName
);
public record FieldSymbol(
    string Name,
    string TypeID,
    string TypeName
);
public record MethodSymbol(
    string Name,
    string ReturnTypeID,
    string ReturnTypeName,
    ParameterItem[] Parameters
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
    IDictionary<string, ErrorSymbol> Errors
);
