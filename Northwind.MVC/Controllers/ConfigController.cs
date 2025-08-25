using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Northwind.MVC.Models;

namespace Northwind.MVC.Controllers;

public class ConfigController : Controller
{
    private readonly IConfigurationRoot _configRoot;
    private readonly NorthwindOptions _options;

    public ConfigController(
        IConfiguration config,
        IOptions<NorthwindOptions> options)
    {
        // No service is registered for IConfigurationRoot but
        // one is registered for IConfiguration and it also
        // implements IConfigurationRoot.
        _configRoot = (IConfigurationRoot)config;

        _options = options.Value;
    }

    public IActionResult Index()
    {
        ConfigIndexViewModel model = new(
            Providers: _configRoot.Providers
                .Select(provider => provider.ToString()),
            Settings: _configRoot.AsEnumerable().ToDictionary(),
                OutputCachingLoggingLevel: _configRoot[
                    "Logging:LogLevel:Microsoft.AspNetCore.OutputCaching"]
                    ?? "Not found.",
            IdentityConnectionString: _configRoot[
                "ConnectionStrings:DefaultConnection"]
                ?? "Not found.",
            Options: _options);

        return View(model);
    }
}
