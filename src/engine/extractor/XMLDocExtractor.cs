
using System.Xml;
using System.Xml.Linq;
using Shiron.Docs.Engine.Model;

namespace Shiron.Docs.Engine.Extractor;

public static class XMLDocExtractor {
    public static IDocumentationToken Get(XNode? node) {
        if (node == null) {
            return new TextToken("");
        }

        if (node is XText t) {
            return new TextToken(t.Value.Trim());
        }

        if (node is not XElement element) {
            return new EmptyToken();
        }

        System.Console.WriteLine($"Getting tokens for element: {element.Name}");
        var tokens = Parse(element);

        return tokens.Items.Length > 1 ? tokens : tokens.Items[0];
    }

    public static ContainerToken Parse(XNode element) {
        if (element is XText n) {
            return new ContainerToken([new TextToken(n.Value.Trim())]);
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
                        case "item":
                            tokens.Add(Get(el));
                            break;
                        default:
                            tokens.Add(new TextToken(el.Value.Trim()));
                            break;
                    }

                    continue;
                }
            }
        }

        return new ContainerToken([.. tokens]);
    }


    public static ListToken ParseList(XElement e) {
        var items = new List<IDocumentationToken>();
        foreach (var n in e.Nodes()) {
            items.Add(Get(n));
        }
        return new ListToken(ListType.Bullet, [.. items]);
    }
}
