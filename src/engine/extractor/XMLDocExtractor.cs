
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

    private static string GetShortLabelForRef(string cref) {
        if (cref.Length > 2 && cref[1] == ':') {
            cref = cref[2..];
        }

        var parenIndex = cref.IndexOf('(');
        if (parenIndex > 0) {
            cref = cref[..parenIndex];
        }

        return cref.Split('.').Last();
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
                        case "see":
                            var cref = el.Attribute("cref")?.Value;
                            var href = el.Attribute("href")?.Value;

                            if (cref == null && href == null) {
                                tokens.Add(new CodeToken(el.Value.Trim()) {
                                    Inline = true
                                });
                                break;
                            }

                            // Primarily handle cref references
                            if (cref != null) {
                                if (cref.StartsWith("!:")) {
                                    var uri = Uri.TryCreate(cref[2..], UriKind.Absolute, out var result) ? result : null;
                                    if (uri != null) {
                                        tokens.Add(new SeeToken(true, uri.ToString(), uri.ToString()) {
                                            IsExternal = true
                                        });
                                    } else {
                                        tokens.Add(new CodeToken(GetShortLabelForRef(cref)) {
                                            Inline = true
                                        });
                                    }

                                    break;
                                }

                                // We know it's an internal reference
                                tokens.Add(new SeeToken(false, cref, GetShortLabelForRef(cref)) {
                                    IsExternal = false
                                });

                                break;
                            }

                            if (href != null) {
                                var uri = Uri.TryCreate(href, UriKind.Absolute, out var result) ? result : null;
                                if (uri == null) {
                                    tokens.Add(new CodeToken(el.Value.Trim()) {
                                        Inline = true
                                    });
                                } else {
                                    tokens.Add(new SeeToken(true, uri.ToString(), uri.ToString()) {
                                        IsExternal = true
                                    });
                                }
                                break;
                            }

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
