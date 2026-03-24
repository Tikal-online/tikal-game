using Duende.IdentityServer.Models;
using Identity.Configuration;

namespace Identity;

internal static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
    ];

    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new("tikal-backend")
    ];

    public static IEnumerable<Client> GetClients(ClientConfiguration config)
    {
        return
        [
            // interactive client using code flow + pkce
            new Client
            {
                ClientId = "interactive",
                ClientSecrets = { new Secret(config.Secret.Sha256()) },

                RequireClientSecret = false,
                RequirePkce = true,

                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = config.RedirectUris,
                PostLogoutRedirectUris = config.PostLogoutRedirectUris,
                FrontChannelLogoutUri = config.FrontendChannelLogoutUri,

                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile", "tikal-backend" }
            }
        ];
    }
}