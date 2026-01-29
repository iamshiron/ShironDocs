
namespace Shiron.Docs.Cli.Utils;

public record NpmPackage(string Package, string Version);

public static class NpmUtils {
    private static readonly List<NpmPackage> _dependencies = [
        new("preact", "^10.26.9"),
        new("preact-iso", "^2.11.1"),
        new("preact-render-to-string", "^6.6.5"),
    ];
    private static readonly List<NpmPackage> _devDependencies = [
        new("@preact/preset-vite", "^2.10.2"),
        new("@types/node", "^20.0.0"),
        new("typescript", "^5.9.3"),
        new("vite", "^7.0.4"),
    ];

    public static object GeneratePackageJson() {
        return new {
            @private = true,
            type = "module",
            scripts = new {
                dev = "vite",
                build = "vite build",
                preview = "vite preview"
            },
            dependencies = _dependencies.ToDictionary(d => d.Package, d => d.Version),
            devDependencies = _devDependencies.ToDictionary(d => d.Package, d => d.Version),
            liscense = "Unlicensed"
        };
    }
}
