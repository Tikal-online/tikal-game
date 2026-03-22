using Duende.IdentityServer.Models;

namespace Identity;

public static class Config
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

    public static IEnumerable<Client> Clients =>
    [
        // interactive client using code flow + pkce
        new()
        {
            ClientId = "interactive",
            ClientSecrets = { new Secret("secret".Sha256()) },

            AllowedGrantTypes = GrantTypes.Code,

            RedirectUris = { "https://localhost:44300/signin-oidc" },
            FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
            PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

            AllowOfflineAccess = true,
            AllowedScopes = { "openid", "profile", "tikal-backend" }
        }
    ];
}