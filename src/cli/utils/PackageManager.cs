
using System.Diagnostics;
using Shiron.Docs.Cli.Exceptions;

namespace Shiron.Docs.Cli.Utils;

public interface IPackageManager {
    string Command { get; }

    Task InstallAsync(string? workingDir);
    Task<string> GetVersionAsync();
}

public class NpmPackageManager : IPackageManager {
    public virtual string Command => "npm";

    public async Task InstallAsync(string? workingDir) {
        try {
            _ = await ProcessUtils.RunProcessASync(Command, ["install"], workingDir);
        } catch (Exception) {
            throw;
        }
    }

    public async Task<string> GetVersionAsync() {
        var res = await ProcessUtils.RunProcessASync(Command, ["--version"]);
        return res.StdOut.Trim();
    }
}

public class PnpmPackageManager : NpmPackageManager {
    public override string Command => "pnpm";
}

public static class PackageManagerUtils {
    private static readonly List<IPackageManager> _packageManagers = [
        new PnpmPackageManager(),
        new NpmPackageManager()
    ];

    public static IPackageManager DetectPackageManager() {
        foreach (var pm in _packageManagers) {
            try {
                var version = pm.GetVersionAsync().GetAwaiter().GetResult();
                if (!string.IsNullOrEmpty(version)) {
                    return pm;
                }
            } catch {
            }
        }

        throw new Exception($"No package manager found. Please install one of the supported package managers: {string.Join(", ", _packageManagers.Select(pm => pm.Command))}.");
    }
}
