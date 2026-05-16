using Accounts.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TikalBackend.WebHost.Configuration;

namespace TikalBackend.WebHost.Extensions;

internal static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public void ApplyMigrations()
        {
            using var scope = app.Services.CreateScope();

            var accountsDbContext = scope.ServiceProvider.GetRequiredService<AccountsDbContext>();

            accountsDbContext.Database.Migrate();
        }

        public void UseScalarUi()
        {
            var config =
                app.Configuration.GetSection(IdentityConfiguration.Position).Get<IdentityConfiguration>()
                ?? throw new InvalidOperationException("Identity configuration is required");

            app.MapScalarApiReference(options =>
            {
                options.AddPreferredSecuritySchemes("OAuth2")
                    .AddAuthorizationCodeFlow("OAuth2",
                        flow =>
                        {
                            flow.ClientId = "interactive.confidential";
                            flow.ClientSecret = config.Secret;
                            flow.Pkce = Pkce.Sha256;
                            flow.SelectedScopes = ["openid", "profile", "tikal-api"];
                        });
            }).AllowAnonymous();
        }
    }
}