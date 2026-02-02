
using Shiron.Docs.Engine;
using Shiron.Docs.Engine.PM;
using Shiron.Docs.Engine.Vite;

namespace Shiron.Docs.Cli.Services;

public static class CLIServices {
    public static async Task BootstrapViteServerAsync(Config config) {
        var packageManager = new PNPMPackageManager();
        var viteBootstrap = new ViteBootstrap(new ViteBootstrapOptions() {
            OutputDir = config.OutputDirectory
        });

        await viteBootstrap.BootstrapAsync(packageManager);
    }
    public static async Task GenerateDocumentationAsync() { }
    public static async Task BuildSiteAsync() { }
}
