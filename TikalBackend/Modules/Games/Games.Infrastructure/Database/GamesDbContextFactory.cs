using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Games.Infrastructure.Database;

public sealed class GamesDbContextFactory : IDesignTimeDbContextFactory<GamesDbContext>
{
    public GamesDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<GamesDbContext>()
            .UseNpgsql(options => options.MigrationsHistoryTable(
                HistoryRepository.DefaultTableName,
                GamesDbContext.Schema
            )).Options;

        return new GamesDbContext(options);

    }
}