using Shiron.Docs.Engine.PM;

namespace Shiron.Docs.Engine.Services;

public interface IWebService {
    Task BootstrapAsync(Config config, IPackageManager pm);
    Task GenerateAsync(Config config, IPackageManager pm);
    Task RunAsync(Config config, IPackageManager pm);
    Task BuildAsync(Config config, IPackageManager pm);
}
