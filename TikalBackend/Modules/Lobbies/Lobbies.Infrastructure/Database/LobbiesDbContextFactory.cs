using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Lobbies.Infrastructure.Database;

public sealed class LobbiesDbContextFactory : IDesignTimeDbContextFactory<LobbiesDbContext>
{
    public LobbiesDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<LobbiesDbContext>()
            .UseNpgsql(options => options.MigrationsHistoryTable(
                HistoryRepository.DefaultTableName,
                LobbiesDbContext.Schema
            )).Options;

        return new LobbiesDbContext(options);
    }
}