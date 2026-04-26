using BFF.Data;
using Duende.Bff.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace BFF.Extensions;

internal static class WebApplictionExtensions
{
    extension(WebApplication app)
    {
        public void ApplyMigrations()
        {
            using var scope = app.Services.CreateScope();

            scope.ServiceProvider.GetRequiredService<SessionDbContext>().Database.Migrate();
            scope.ServiceProvider.GetRequiredService<DataProtectionDbContext>().Database.Migrate();
        }
    }
}