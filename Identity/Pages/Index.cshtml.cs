using System.Reflection;
using Duende.IdentityServer;
using Duende.IdentityServer.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Identity.Pages;

[AllowAnonymous]
public class Index : PageModel
{
    public string Version => typeof(IdentityServerMiddleware).Assembly
                                 .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                 ?.InformationalVersion.Split('+')[0]
                             ?? "unavailable";

    public IdentityServerLicense? License { get; }

    public Index(IdentityServerLicense? license = null)
    {
        License = license;
    }
}