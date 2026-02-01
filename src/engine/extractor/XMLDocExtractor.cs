
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Shiron.Docs.Engine.Model;

namespace Shiron.Docs.Engine.Extractor;

public static class XMLDocExtractor {
    public static Dictionary<string, List<string>> MissedTokens = [];

    private static string CleanString(string str) {
        if (string.IsNullOrWhiteSpace(str)) return string.Empty;

        bool hasMultiSpace = false;
        for (int i = 0; i < str.Length - 1; i++) {
            if (char.IsWhiteSpace(str[i]) && char.IsWhiteSpace(str[i + 1])) {
                hasMultiSpace = true;
                break;
            }
        }
        if (!hasMultiSpace && !str.Any(c => c == '\n' || c == '\r' || c == '\t')) {
            return str.Trim();
        }

        var sb = new StringBuilder(str.Length);
        bool lastWasSpace = false;

        foreach (var c in str) {
            if (char.IsWhiteSpace(c)) {
                if (!lastWasSpace) {
                    _ = sb.Append(' ');
                    lastWasSpace = true;
                }
            } else {
                _ = sb.Append(c);
                lastWasSpace = false;
            }
        }

        return sb.ToString().Trim();
    }

    private static TextToken CreateTextToken(string text) {
        return new TextToken(CleanString(text));
    }

    private static CodeToken CreateCodeToken(string rawCode, bool inline) {
        if (inline) {
            return new CodeToken(CleanString(rawCode)) { Inline = true };
        }

        if (string.IsNullOrWhiteSpace(rawCode)) {
            return new CodeToken(string.Empty) { Inline = false };
        }

        var minIndent = int.MaxValue;
        var currentIndent = 0;

        using (var reader = new StringReader(rawCode)) {
            string? line;
            while ((line = reader.ReadLine()) != null) {
                if (string.IsNullOrWhiteSpace(line)) continue;

                currentIndent = 0;
                foreach (char c in line) {
                    if (!char.IsWhiteSpace(c)) break;
                    currentIndent++;
                }
                if (currentIndent < minIndent) minIndent = currentIndent;
            }
        }

        if (minIndent == int.MaxValue) minIndent = 0;

        var sb = new StringBuilder(rawCode.Length);
        using (var reader = new StringReader(rawCode)) {
            string? line;
            bool first = true;
            while ((line = reader.ReadLine()) != null) {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (!first) _ = sb.Append('\n');

                _ = line.Length >= minIndent ? sb.Append(line.AsSpan(minIndent)) : sb.Append(line.Trim());

                first = false;
            }
        }

        return new CodeToken(sb.ToString()) { Inline = false };
    }

    /// <summary>
    /// Get documentation tokens from an XML node.
    /// </summary>
    /// <param name="node">The XML node to extract documentation tokens from.</param>
    /// <returns>An array of documentation tokens extracted from the XML node, or null if the node is null.</returns>
    public static IDocumentationToken[]? GetNull(XNode? node) {
        if (node == null) {
            return null;
        }

        return ParseNode(node);
    }

    /// <summary>
    /// Get documentation tokens from an XML node.
    /// </summary>
    /// <param name="node">The XML node to extract documentation tokens from.</param>
    /// <returns>An array of documentation tokens extracted from the XML node.</returns>
    public static IDocumentationToken[] Get(XNode? node) {
        if (node == null) {
            return [];
        }

        return ParseNode(node);
    }

    public static Dictionary<string, IDocumentationToken[]> GetParam(IEnumerable<XElement> node) {
        var res = new Dictionary<string, IDocumentationToken[]>();

        foreach (var param in node) {
            var nameAttr = param.Attribute("name");
            if (nameAttr == null) {
                continue;
            }

            res[nameAttr.Value] = ParseNode(param);
        }
        return res;
    }

    public static IDocumentationToken[] ParseNode(XNode? node) {
        if (node == null) {
            return [new TextToken(string.Empty)];
        }

        if (node is XText t) {
            return [CreateTextToken(t.Value.Trim())];
        }

        if (node is not XElement element) {
            return [];
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
            return [CreateTextToken(n.Value.Trim())];
        }

        var tokens = new List<IDocumentationToken>();
        if (element is XElement e) {
            foreach (var node in e.Nodes()) {
                if (node is XText text) {
                    tokens.Add(CreateTextToken(text.Value.Trim()));
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
                        case "strong" or "b" or "bold":
                            tokens.Add(new StyleToken(
                                ParseNode(el),
                                StyleType.Bold
                            ));
                            break;
                        case "em" or "i" or "italic":
                            tokens.Add(new StyleToken(
                                ParseNode(el),
                                StyleType.Italic
                            ));
                            break;
                        case "underline" or "u":
                            tokens.Add(new StyleToken(
                                ParseNode(el),
                                StyleType.Underline
                            ));
                            break;
                        case "strikethrough" or "s":
                            tokens.Add(new StyleToken(
                                ParseNode(el),
                                StyleType.Strikethrough
                            ));
                            break;
                        case "c":
                            tokens.Add(CreateCodeToken(el.Value, true));
                            break;
                        case "code":
                            tokens.Add(CreateCodeToken(el.Value, false));
                            break;
                        case "description":
                            tokens.Add(PackTokens(ParseNode(el)));
                            break;
                        case "see":
                            var cref = el.Attribute("cref")?.Value;
                            var href = el.Attribute("href")?.Value;

                            if (cref == null && href == null) {
                                tokens.Add(CreateCodeToken(el.Value, true));
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
                                        tokens.Add(CreateCodeToken(GetShortLabelForRef(cref), true));
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
                                    tokens.Add(CreateCodeToken(href, true));
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
        var items = new List<IDocumentationToken[]>();
        foreach (var n in e.Nodes()) {
            if (n is XElement element) {
                items.Add(ParseListItem(element));
                continue;
            }


            var token = Get(n);
            if (token != null) {
                items.Add(token);
            }
        }
        return new ListToken(ListType.Bullet, [.. items]);
    }

    public static IDocumentationToken[] ParseListItem(XElement e) {
        var name = e.Name.LocalName;
        if (name == "item") {
            var termElement = e.Element("term");
            var descriptionElement = e.Element("description");

            if (termElement != null && descriptionElement != null) {
                var key = termElement.Value.Trim();
                var value = ParseNode(descriptionElement);
                return [new KVPListItem(key, [.. value])];
            } else {
                return ParseNode(e);
            }
        } else {
            return Get(e);
        }
    }
}
