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
        new("tikal-api")
    ];

    public static IEnumerable<Client> GetClients(ClientConfiguration config)
    {
        return
        [
            new Client
            {
                ClientId = "interactive.confidential",
                ClientSecrets = { new Secret(config.Secret.Sha256()) },

                RequireClientSecret = true,
                RequirePkce = true,

                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris =
                [
                    $"{config.BffUrl}/signin-oidc",
                    $"{config.BackendUrl}/scalar/v1"
                ],
                PostLogoutRedirectUris = [$"{config.BffUrl}/signout-callback-oidc"],
                FrontChannelLogoutUri = $"{config.BffUrl}/signout-oidc",

                AllowedCorsOrigins =
                [
                    config.BackendUrl
                ],

                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile", "tikal-api" }
            }
        ];
    }
}