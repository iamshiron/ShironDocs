
using System.Text.Json;
using Shiron.Docs.Engine.PM;

namespace Shiron.Docs.Engine.Vite;

public class ViteBootstrapOptions {
    public string OutputDir { get; set; } = "_site";
}

public class ViteBootstrap {
    public readonly ViteBootstrapOptions Options;

    private readonly List<PackageDependency> _dependencies = [
        new PackageDependency("@phosphor-icons/react", "^2.1.10"),
        new PackageDependency("preact", "^10.28.2"),
        new PackageDependency("preact-iso", "^2.11.1"),
        new PackageDependency("preact-router", "^4.1.2")
    ];
    private readonly List<PackageDependency> _devDependencies = [
        new PackageDependency("@biomejs/biome", "^2.3.13"),
        new PackageDependency("@preact/preset-vite", "^2.10.3"),
        new PackageDependency("@tailwindcss/postcss", "^4.1.18"),
        new PackageDependency("@tailwindcss/typography", "^0.5.19"),
        new PackageDependency("autoprefixer", "^10.4.23"),
        new PackageDependency("postcss", "^8.5.6"),
        new PackageDependency("preact-render-to-string", "^6.6.5"),
        new PackageDependency("tailwindcss", "^4.1.18"),
        new PackageDependency("typescript", "^5.9.3"),
        new PackageDependency("vite", "^7.3.1"),
        new PackageDependency("vite-plugin-pages", "^0.33.2")
    ];

    public ViteBootstrap(ViteBootstrapOptions options) {
        Options = options;
    }

    public object CreatePackageJSON() {
        return new {
            @name = "shirondocs-site",
            @private = true,
            @version = "0.0.0",
            @type = "module",
            @dependencies = _dependencies.ToDictionary(dep => dep.Name, dep => dep.Version),
            @devDependencies = _devDependencies.ToDictionary(dep => dep.Name, dep => dep.Version)
        };
    }

    public async Task BootstrapAsync(IPackageManager pm) {
        if (!Directory.Exists(Options.OutputDir)) {
            Directory.CreateDirectory(Options.OutputDir);
        }

        var root = Path.Join(Directory.GetCurrentDirectory(), Options.OutputDir);
        var json = CreatePackageJSON();
        await File.WriteAllTextAsync(Path.Join(root, "package.json"), JsonSerializer.Serialize(json, new JsonSerializerOptions {
            WriteIndented = true
        }));

        await pm.InstallPackagesAsync(Path.Join(Directory.GetCurrentDirectory(), Options.OutputDir));
    }
}
