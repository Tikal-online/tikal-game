using Games.Application;
using Games.Application.DataAccess;
using Games.Infrastructure.Database;
using Games.Infrastructure.QueryContexts;
using Games.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;

namespace Games.Infrastructure;

public static class GamesInfrastructureModule
{
    extension(IServiceCollection services)
    {
        public void AddGamesInfrastructure(string connectionString)
        {
            services.ConfigureWolverine(opts =>
            {
                opts.Discovery.IncludeAssembly(AssemblyReference.Assembly);

                opts.CodeGeneration.AlwaysUseServiceLocationFor<GamesDbContext>();
            });

            services.AddDbContext<GamesDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseNpgsql(
                    connectionString,
                    options => options.MigrationsHistoryTable(
                        HistoryRepository.DefaultTableName,
                        GamesDbContext.Schema
                    ));
            });

            services.AddScoped<GameRepository, DbGameRepository>();

            services.AddScoped<GameQueryContext, DbGameQueryContext>();
        }
    }
}