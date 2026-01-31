
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shiron.Docs.Engine.Model;

public enum StyleType {
    Bold,
    Italic,
    Underline,
    Strikethrough
}

[JsonDerivedType(typeof(ContainerToken), "container")]
[JsonDerivedType(typeof(ParagraphToken), "paragraph")]
[JsonDerivedType(typeof(CodeToken), "code")]
[JsonDerivedType(typeof(TextToken), "text")]
[JsonDerivedType(typeof(SeeToken), "see")]
[JsonDerivedType(typeof(ListToken), "list")]
[JsonDerivedType(typeof(RemarksToken), "remarks")]
[JsonDerivedType(typeof(KVPListItem), "kvp")]
[JsonDerivedType(typeof(StyleToken), "style")]
public interface IDocumentationToken {
}

public record KVPListItem(
    string Key,
    IDocumentationToken[] Value
) : IDocumentationToken;

public record ContainerToken(
    IDocumentationToken[] Items
) : IDocumentationToken;

public record ParagraphToken(
    IDocumentationToken[] Items
) : IDocumentationToken;

public record CodeToken(
    string Code
) : IDocumentationToken {
    public bool? Inline { get; init; } = null;
}

public record TextToken(
    string Text
) : IDocumentationToken;

public record StyleToken(
    IDocumentationToken[] Items,
    StyleType StyleType
) : IDocumentationToken;

public record SeeToken(
    bool IsExternal,
    string ReferenceID,
    string DisplayText
) : IDocumentationToken;

public enum ListType {
    Bullet,
    Numbered
}

public record ListToken(
    ListType Type,
    IDocumentationToken[][] Items
) : IDocumentationToken;

public record RemarksToken(
    IDocumentationToken[] Content
) : IDocumentationToken;
