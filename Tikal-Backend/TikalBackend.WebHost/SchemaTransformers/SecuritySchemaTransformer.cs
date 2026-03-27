using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using TikalBackend.WebHost.Configuration;

namespace TikalBackend.WebHost.SchemaTransformers;

internal sealed class SecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    private readonly IdentityConfiguration configuration;

    public SecuritySchemeTransformer(IOptions<IdentityConfiguration> options)
    {
        configuration = options.Value;
    }

    public Task TransformAsync(OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            ["OAuth2"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{configuration.Authority}/connect/authorize"),
                        TokenUrl = new Uri($"{configuration.Authority}/connect/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "OpenID" },
                            { "profile", "Profile" },
                            { "tikal-backend", "Access the API" }
                        }
                    }
                }
            }
        };

        return Task.CompletedTask;
    }
}