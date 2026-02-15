using Shiron.Docs.Engine.utils;

namespace Shiron.Docs.Engine.PM;

public interface IPackageManager {
    string Command { get; }

    Task InstallAsync(string? workingDir);
    Task<string> GetVersionAsync();
}

public class NpmPackageManager : IPackageManager {
    public virtual string Command => "npm";

    public async Task InstallAsync(string? workingDir) {
        _ = await ShellUtils.RunAsync(Command, ["install"], workingDir);
    }

    public async Task<string> GetVersionAsync() {
        try {
            var res = await ShellUtils.RunAsync(Command, ["--version"], null);
            return res.StdOut.Trim();
        } catch (Exception e) {
            Console.WriteLine($"Could not get pm version for {Command}");
            Console.WriteLine(e);
            throw;
        }
    }
}

public class PnpmPackageManager : NpmPackageManager {
    public override string Command => "pnpm";
}

public static class PackageManagerUtils {
    private static readonly List<IPackageManager> PackageManagers = [
        new PnpmPackageManager(),
        new NpmPackageManager()
    ];

    /// <summary>
    /// Detects and returns the first available package manager based on the environment.
    /// Defaults to a specific package manager if a hint is provided.
    /// </summary>
    /// <param name="hint">Optional hint specifying the package manager to prioritize.</param>
    /// <returns>The detected package manager implementing IPackageManager.</returns>
    /// <exception cref="Exception">Thrown if no supported package manager is found.</exception>
    public static async Task<IPackageManager> DetectPackageManager(string? hint = null) {
        foreach (var pm in PackageManagers) {
            if (hint != null && string.Equals(pm.Command, hint, StringComparison.OrdinalIgnoreCase)) {
                continue;
            }

            try {
                Console.WriteLine($"Checking pm {pm.Command}...");
                var version = await pm.GetVersionAsync();
                if (!string.IsNullOrEmpty(version)) {
                    return pm;
                }
            } catch {
                // Do not print exception, it is meant to fail if the package manager is not installed
                // If none is found, an exception will be thrown below
            }
        }

        throw new Exception(
            $"No package manager found. Please install one of the supported package managers: {string.Join(", ", PackageManagers.Select(pm => pm.Command))}.");
    }
}
