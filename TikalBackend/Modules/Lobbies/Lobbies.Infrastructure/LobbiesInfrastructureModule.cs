using Lobbies.Application.DataAccess;
using Lobbies.Infrastructure.Database;
using Lobbies.Infrastructure.QueryContexts;
using Lobbies.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace Lobbies.Infrastructure;

public static class LobbiesInfrastructureModule
{
    extension(IServiceCollection services)
    {
        public void AddLobbiesInfrastructure(string connectionString)
        {
            services.AddDbContext<LobbiesDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseNpgsql(
                    connectionString,
                    options => options.MigrationsHistoryTable(
                        HistoryRepository.DefaultTableName,
                        LobbiesDbContext.Schema
                    )
                );
            });

            services.AddScoped<LobbyRepository, DbLobbyRepository>();
            services.AddScoped<PlayerRepository, DbPlayerRepository>();

            services.AddScoped<LobbyQueryContext, DbLobbyQueryContext>();
            services.AddScoped<PlayerQueryContext, DbPlayerQueryContext>();

            services.AddScoped<UnitOfWork>(sp => sp.GetRequiredService<LobbiesDbContext>());
        }
    }
}