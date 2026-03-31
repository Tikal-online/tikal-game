using Accounts.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace TikalBackend.WebHost.Extensions;

internal static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public void ApplyMigrations()
        {
            using var scope = app.Services.CreateScope();

            var usersDbContext = scope.ServiceProvider.GetRequiredService<AccountsDbContext>();

            usersDbContext.Database.Migrate();
        }

        public void UseScalarUi()
        {
            app.MapScalarApiReference(options =>
            {
                options.AddPreferredSecuritySchemes("OAuth2")
                    .AddAuthorizationCodeFlow("OAuth2",
                        flow =>
                        {
                            flow.ClientId = "interactive";
                            flow.Pkce = Pkce.Sha256;
                            flow.SelectedScopes = ["openid", "profile", "tikal-backend"];
                        });
            }).AllowAnonymous();
        }
    }
}