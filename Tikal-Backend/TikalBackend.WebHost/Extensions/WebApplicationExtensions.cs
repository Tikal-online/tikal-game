using Accounts.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

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
    }
}