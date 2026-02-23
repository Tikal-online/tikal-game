using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Users.Application;
using Users.Infrastructure.Database;
using Users.Infrastructure.Entities;

namespace Users.Infrastructure;

public static class UsersModule
{
    extension(IServiceCollection services)
    {
        public void AddUsersInfrastructure(string connectionString)
        {
            services.AddDbContext<UsersDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseNpgsql(
                    connectionString,
                    o => o.MigrationsHistoryTable(
                        HistoryRepository.DefaultTableName,
                        UsersDbContext.Schema
                    ));
            });

            services.AddIdentity<UserEntity, IdentityRole<int>>()
                .AddEntityFrameworkStores<UsersDbContext>();

            services.AddSingleton<UserRepository, IdentityUserRepository>();
        }
    }
}