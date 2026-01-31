
using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis;

namespace Shiron.Docs.Engine.Model;

public interface ISymbolContainer {
    ConcurrentBag<string> ChildIDs { get; }
}

public record ParameterItem(
    string Name,
    string TypeID,
    string TypeName,
    IDocumentationToken[]? Documentation
);

public record EnumItem(
    string Name,
    string Value,
    string ID,
    IDocumentationToken[]? Summary,
    IDocumentationToken[]? Remarks
);

public record NamespaceSymbol(
    string Name
) : ISymbolContainer {
    public ConcurrentBag<string> ChildIDs { get; } = [];
}

[JsonDerivedType(typeof(TypeSymbol), "base")]
[JsonDerivedType(typeof(EnumSymbol), "enum")]
public record TypeSymbol(
    string Name,
    ConcurrentBag<string> ChildIDs,
    IDocumentationToken[]? Summary,
    IDocumentationToken[]? Remarks
) : ISymbolContainer;

public record EnumSymbol(
    string Name,
    string UnderlyingTypeID,
    string UnderlyingTypeName,
    EnumItem[] Options,
    IDocumentationToken[]? Summary,
    IDocumentationToken[]? Remarks
) : TypeSymbol(Name, [], Summary, Remarks);

public record PropertySymbol(
    string Name,
    string TypeID,
    string TypeName,
    IDocumentationToken[]? Summary,
    IDocumentationToken[]? Remarks
);
public record FieldSymbol(
    string Name,
    string TypeID,
    string TypeName,
    IDocumentationToken[]? Summary,
    IDocumentationToken[]? Remarks
);
public record MethodSymbol(
    string Name,
    string ReturnTypeID,
    string ReturnTypeName,
    ParameterItem[] Parameters,
    IDocumentationToken[]? Summary,
    IDocumentationToken[]? Remarks
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
