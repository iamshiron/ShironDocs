
using Shiron.Lib.Utils;

namespace Shiron.Docs.Engine.PM;

public record PackageDependency(string Name, string Version);

public interface IPackageManager {
    public string Command { get; }

    public Task<string> GetVersionAsync();
    public Task InstallPackagesAsync(string? workingDir);
}

public class NPMPackageManager : IPackageManager {
    public virtual string Command => "npm";

    public async Task<string> GetVersionAsync() {
        var res = ShellUtils.Run(Command, ["--version"], null, null);
        if (res.ExitCode != 0) {
            throw new Exception($"Failed to get npm version: {res.StdErr}");
        }
        return res.StdOut.Trim();
    }

    public async Task InstallPackagesAsync(string? workingDir = null) {
        var res = ShellUtils.Run(Command, ["install"], workingDir, null);
        if (res.ExitCode != 0) {
            throw new Exception($"Failed to install npm packages: {res.StdErr}");
        }
    }
}

public class PNPMPackageManager : NPMPackageManager {
    public override string Command => "pnpm";
}
