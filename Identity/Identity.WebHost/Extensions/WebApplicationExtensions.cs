using Microsoft.EntityFrameworkCore;
using Users.Infrastructure.Database;

namespace Identity.WebHost.Extensions;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public void ApplyMigrations()
        {
            using var scope = app.Services.CreateScope();

            var usersDbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

            usersDbContext.Database.Migrate();
        }
    }
}