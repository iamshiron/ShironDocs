using System.Text.Json;
using Shiron.Docs.Engine.PM;

namespace Shiron.Docs.Engine.Vite;

public readonly record struct PackageDependency(string Package, string Version);

public class ViteBootstrapOptions {
    public string OutputDir { get; set; } = "_site";
}

public class ViteBootstrap {
    public readonly ViteBootstrapOptions Options;

    private readonly List<PackageDependency> _dependencies = [
        new("@phosphor-icons/react", "^2.1.10"),
        new("preact", "^10.28.2"),
        new("preact-iso", "^2.11.1"),
        new("preact-router", "^4.1.2")
    ];
    private readonly List<PackageDependency> _devDependencies = [
        new("@biomejs/biome", "^2.3.13"),
        new("@preact/preset-vite", "^2.10.3"),
        new("@tailwindcss/postcss", "^4.1.18"),
        new("@tailwindcss/typography", "^0.5.19"),
        new("autoprefixer", "^10.4.23"),
        new("postcss", "^8.5.6"),
        new("preact-render-to-string", "^6.6.5"),
        new("tailwindcss", "^4.1.18"),
        new("typescript", "^5.9.3"),
        new("vite", "^7.3.1"),
        new("vite-plugin-pages", "^0.33.2")
    ];

    public ViteBootstrap(ViteBootstrapOptions options) {
        Options = options;
    }

    public object CreatePackageJSON() {
        return new {
            name = "shirondocs-site",
            @private = true,
            version = "0.0.0",
            type = "module",
            dependencies = _dependencies.ToDictionary(dep => dep.Package, dep => dep.Version),
            devDependencies = _devDependencies.ToDictionary(dep => dep.Package, dep => dep.Version)
        };
    }

    public async Task BootstrapAsync(IPackageManager pm) {
        if (!Directory.Exists(Options.OutputDir)) Directory.CreateDirectory(Options.OutputDir);

        var root = Path.Join(Directory.GetCurrentDirectory(), Options.OutputDir);
        var json = CreatePackageJSON();
        await File.WriteAllTextAsync(Path.Join(root, "package.json"), JsonSerializer.Serialize(json, new JsonSerializerOptions {
            WriteIndented = true
        }));

        await pm.InstallAsync(Path.Join(Directory.GetCurrentDirectory(), Options.OutputDir));
    }
}
