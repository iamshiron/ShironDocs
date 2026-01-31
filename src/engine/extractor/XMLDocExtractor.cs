
using System.Xml;
using System.Xml.Linq;
using Shiron.Docs.Engine.Model;

namespace Shiron.Docs.Engine.Extractor;

public static class XMLDocExtractor {
    public static Dictionary<string, List<string>> MissedTokens = [];

    public static IDocumentationToken Get(XNode? node) {
        return PackTokens(ParseNode(node));
    }

    public static IDocumentationToken[] ParseNode(XNode? node) {
        if (node == null) {
            return [new TextToken("")];
        }

        if (node is XText t) {
            return [new TextToken(t.Value.Trim())];
        }

        if (node is not XElement element) {
            return [new EmptyToken()];
        }

        var tokens = Parse(element);
        return tokens;
    }

    private static IDocumentationToken PackTokens(IDocumentationToken[] tokens) {
        if (tokens.Length == 1) {
            return tokens[0];
        }
        return new ContainerToken([.. tokens]);
    }

    public static IDocumentationToken[] Parse(XNode element) {
        if (element is XText n) {
            return [new TextToken(n.Value.Trim())];
        }

        var tokens = new List<IDocumentationToken>();
        if (element is XElement e) {
            foreach (var node in e.Nodes()) {
                if (node is XText text) {
                    tokens.Add(new TextToken(text.Value.Trim()));
                    continue;
                }

                if (node is XElement el) {
                    switch (el.Name.LocalName) {
                        case "list":
                            tokens.Add(ParseList(el));
                            break;
                        case "para":
                            tokens.Add(new ParagraphToken(ParseNode(el)));
                            break;
                        case "strong":
                            tokens.Add(new TextToken(el.Value.Trim()) {
                                Bold = true
                            });
                            break;
                        case "c":
                            tokens.Add(new CodeToken(el.Value.Trim()) {
                                Inline = true
                            });
                            break;
                        case "code":
                            tokens.Add(new CodeToken(el.Value.Trim()) {
                                Inline = false
                            });
                            break;
                        case "description":
                            tokens.Add(PackTokens(ParseNode(el)));
                            break;
                        default:
                            if (MissedTokens.ContainsKey(el.Name.LocalName)) {
                                MissedTokens[el.Name.LocalName].Add(el.ToString());
                            } else {
                                MissedTokens[el.Name.LocalName] = [el.ToString()];
                            }
                            break;
                    }

                    continue;
                }
            }
        }

        return [.. tokens];
    }


    public static ListToken ParseList(XElement e) {
        var items = new List<IDocumentationToken>();
        foreach (var n in e.Nodes()) {
            if (n is XElement element) {
                items.Add(ParseListItem(element));
                continue;
            }

            items.Add(Get(n));
        }
        return new ListToken(ListType.Bullet, [.. items]);
    }

    public static IDocumentationToken ParseListItem(XElement e) {
        var name = e.Name.LocalName;
        if (name == "item") {
            var termElement = e.Element("term");
            var descriptionElement = e.Element("description");

            if (termElement != null && descriptionElement != null) {
                var key = termElement.Value.Trim();
                var value = ParseNode(descriptionElement);
                return new KVPListItem(key, [.. value]);
            } else {
                return PackTokens(ParseNode(e));
            }
        } else {
            return Get(e);
        }
    }
}
