
namespace Shiron.Docs.Engine.Model;

public enum SymbolKind {
    Namespace, Class, Interface, Struct, Enum, Method, Property, Field
}
public enum TypeFlags {
    None = 0,
    Abstract = 1 << 0,
    Sealed = 1 << 1,
    Static = 1 << 2,
    Readonly = 1 << 3,
    Partial = 1 << 4
}
public enum MethodFlags {
    None = 0,
    Static = 1 << 0,
    Abstract = 1 << 1,
    Virtual = 1 << 2,
    Override = 1 << 3,
    Async = 1 << 4
}
public enum AccessFlags {
    None = 0,
    Public = 1 << 0,
    Private = 1 << 1,
    Protected = 1 << 2,
    Internal = 1 << 3
}

public record DocumentationSymbol(
    string ID,
    SymbolKind Kind,
    string DisplayName,
    string Name,
    string FullName,
    AccessFlags Access,
    List<string> ChildrenIDs
);

public record ParameterData(
    string Name,
    string TypeID
);
public record MethodData(
    string ID,
    string ReturnTypeID,
    List<ParameterData> Parameters,
    MethodFlags Flags
);

public record AssemblyData(
    string Name,
    string Version,
    string CSProjFile,
    List<DocumentationSymbol> Symbols,
    List<MethodData> MethodMetadata
);
