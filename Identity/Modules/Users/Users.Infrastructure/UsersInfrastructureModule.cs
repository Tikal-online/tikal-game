using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Users.Application;
using Users.Domain.Entities;
using Users.Infrastructure.Database;

namespace Users.Infrastructure;

public static class UsersInfrastructureModule
{
    extension(IServiceCollection services)
    {
        public void AddUsersInfrastructure(string connectionString)
        {
            services.AddDbContext<UsersDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseNpgsql(
                    connectionString,
                    options => options.MigrationsHistoryTable(
                        HistoryRepository.DefaultTableName,
                        UsersDbContext.Schema));
            });

            services.AddIdentity<User, IdentityRole<int>>(options =>
                {
                    options.User.AllowedUserNameCharacters = string.Empty;
                })
                .AddEntityFrameworkStores<UsersDbContext>();

            services.AddScoped<UserRepository, IdentityUserRepository>();
        }
    }
}