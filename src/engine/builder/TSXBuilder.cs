
using Shiron.Docs.Engine.Model;

namespace Shiron.Docs.Engine.Builder;

public class TSXBuilder {
    private readonly string _outputRoot;

    public TSXBuilder(string outputRoot) {
        _outputRoot = outputRoot;
        if (!Directory.Exists(_outputRoot)) {
            Directory.CreateDirectory(_outputRoot);
        }
    }

    private string SanitizeFileName(string name) {
        foreach (var c in Path.GetInvalidFileNameChars()) {
            name = name.Replace(c, '_');
        }
        return name;
    }

    public async Task BuildAssemblyAsync(AssemblyData assembly) {
        foreach (var (docID, type) in assembly.Types) {
            var outputFile = Path.Combine(_outputRoot, $"{SanitizeFileName(docID)}.tsx");

            using var writer = new StreamWriter(outputFile);
            await writer.WriteLineAsync($"// Auto-generated file for type: {type.Name}");

            writer.Close();
        }
    }
}
