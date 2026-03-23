using Accounts.Application.DataAccess;
using Accounts.Infrastructure.Database;
using Accounts.Infrastructure.QueryContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace Accounts.Infrastructure;

public static class AccountsInfrastructureModule
{
    extension(IServiceCollection services)
    {
        public void AddAccountsInfrastructure(string connectionString)
        {
            services.AddDbContext<AccountsDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseNpgsql(
                    connectionString,
                    options => options.MigrationsHistoryTable(
                        HistoryRepository.DefaultTableName,
                        AccountsDbContext.Schema
                    )
                );
            });

            services.AddScoped<AccountQueryContext, DbAccountQueryContext>();

            services.AddScoped<UnitOfWork>(sp => sp.GetRequiredService<AccountsDbContext>());
        }
    }
}