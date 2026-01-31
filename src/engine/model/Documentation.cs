
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shiron.Docs.Engine.Model;

public enum TokenType {
    Container,
    Text,
    See,
    List,
    Remarks,
    Empty
}

[JsonConverter(typeof(TokenTypeConverter))]
public class TokenTypeConverter : JsonConverter<TokenType> {
    public override TokenType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        throw new Exception("No need for deserialization of TokenType");
    }

    public override void Write(Utf8JsonWriter writer, TokenType value, JsonSerializerOptions options) {
        writer.WriteStringValue(value.ToString());
    }

}

[JsonDerivedType(typeof(ContainerToken), "container")]
[JsonDerivedType(typeof(TextToken), "text")]
[JsonDerivedType(typeof(SeeToken), "see")]
[JsonDerivedType(typeof(ListToken), "list")]
[JsonDerivedType(typeof(RemarksToken), "remarks")]
[JsonDerivedType(typeof(EmptyToken), "empty")]
public interface IDocumentationToken {
    TokenType TokenType { get; }
}

public record EmptyToken() : IDocumentationToken {
    public TokenType TokenType => TokenType.Empty;
}

public record ContainerToken(
    IDocumentationToken[] Items
) : IDocumentationToken {
    public TokenType TokenType => TokenType.Container;
}

public record TextToken(
    string Text
) : IDocumentationToken {
    public TokenType TokenType => TokenType.Text;
}

public record SeeToken(
    bool IsExternal,
    string ReferenceID
) : IDocumentationToken {
    public TokenType TokenType => TokenType.See;
}

public enum ListType {
    Bullet,
    Numbered
}

public record ListToken(
    ListType Type,
    IDocumentationToken[] Items
) : IDocumentationToken {
    public TokenType TokenType => TokenType.List;
}

public record RemarksToken(
    IDocumentationToken[] Content
) : IDocumentationToken {
    public TokenType TokenType => TokenType.Remarks;
}
