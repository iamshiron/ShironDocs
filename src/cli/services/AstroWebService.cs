using System.Text.Json;
using Shiron.Docs.Engine;
using Shiron.Docs.Engine.PM;
using Shiron.Docs.Engine.Services;

namespace Shiron.Docs.Cli.Services;

public record PackageDependency(string Package, string Version);

public class AstroWebService(string workingDir) : IWebService {
    private readonly string _workingDir = workingDir;

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private static readonly PackageDependency[] Dependencies = [
        new("@astrojs/preact", "^4.1.3"),
        new("@tailwindcss/vite", "^4.2.0"),
        new("astro", "^5.17.1"),
        new("preact", "^10.28.4"),
        new("tailwindcss", "^4.2.0")
    ];

    private string Join(params string[] path) {
        return Path.Join([_workingDir, ..path]);
    }

    public async Task BootstrapAsync(Config config, IPackageManager pm) {
        var packageJson = new {
            name = config.AppName.ToLower().Replace(" ", "-"),
            type = "module",
            version = config.AppVersion,
            @private = true,
            scripts = new {
                dev = "astro dev",
                build = "astro build",
                preview = "astro preview",
                astro = "astro"
            },
            dependencies = Dependencies.ToDictionary(d => d.Package, d => d.Version)
        };

        await File.WriteAllTextAsync(Join("package.json"), JsonSerializer.Serialize(packageJson, JsonOptions));

        Directory.CreateDirectory(Join("src/pages"));
        Directory.CreateDirectory(Join("src/layouts"));
        Directory.CreateDirectory(Join("src/styles"));

        // Setup base files
        await File.WriteAllTextAsync(Join("tsconfig.json"), JsonSerializer.Serialize(new {
            extends = "astro/tsconfig/strict",
            includes = new[] {
                ".astro/types.d.ts",
                "**/*"
            },
            excludes = new[] {
                "dist"
            },
            compilerOptions = new {
                jsx = "react-jsx",
                jsxImportSource = "preact"
            }
        }, JsonOptions));

        await File.WriteAllTextAsync(Join("astro.config.mjs"),
            """
            // @ts-check
            import { defineConfig } from 'astro/config';

            import preact from '@astrojs/preact';

            import tailwindcss from '@tailwindcss/vite';

            // https://astro.build/config
            export default defineConfig({
                integrations: [preact()],
                vite: {
                    plugins: [tailwindcss()]
                }
            });
            """
        );

        // Write source files
        await File.WriteAllTextAsync(Join("src/layouts/Layout.astro"),
            """
            ---
            import "../styles/global.css";
            ---
            """
        );

        await File.WriteAllTextAsync(Join("src/styles/global.css"),
            """
            @import "tailwindcss";
            """
        );

        await File.WriteAllTextAsync(Join("src/pages/index.astra"),
            """
            ---

            ---

            <html lang="en">
            	<head>
            		<meta charset="utf-8" />
            		<link rel="icon" type="image/svg+xml" href="/favicon.svg" />
            		<link rel="icon" href="/favicon.ico" />
            		<meta name="viewport" content="width=device-width" />
            		<meta name="generator" content={Astro.generator} />
            		<title>Astro</title>
            	</head>
            	<body>
            		<h1>Astro</h1>
            	</body>
            </html>
            """
        );
    }

    public Task GenerateAsync(Config config, IPackageManager pm) {
        throw new NotImplementedException();
    }

    public Task RunAsync(Config config, IPackageManager pm) {
        throw new NotImplementedException();
    }

    public async Task BuildAsync(Config config, IPackageManager pm) {
        await pm.RunAsync(["build"], _workingDir);
    }
}
