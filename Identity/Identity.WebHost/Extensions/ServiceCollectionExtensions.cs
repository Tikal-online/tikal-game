using FluentValidation;
using Identity.WebHost.Configuration;
using Identity.WebHost.ExceptionHandlers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Shared.Application.Pipelines;
using Users.Application;
using Users.Infrastructure;
using Users.Infrastructure.Database;
using Users.Infrastructure.Entities;

namespace Identity.WebHost.Extensions;

internal static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddMediatR()
        {
            services.AddMediatR(c =>
            {
                c.RegisterServicesFromAssemblies(AssemblyReference.Assembly);

                c.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationPipeline<,>));
            });
        }

        public void AddValidators()
        {
            services.AddValidatorsFromAssemblies([AssemblyReference.Assembly]);
        }

        public void AddInfrastructure()
        {
            services.AddSingleton<UserRepository, IdentityUserRepository>();
        }

        public void AddProdDbContext(IConfiguration configuration)
        {
            var options = new DatabaseOptions();
            configuration.GetSection(DatabaseOptions.Position).Bind(options);

            services.AddDbContext<UsersDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseNpgsql(
                    $"Server={options.Host};" +
                    $"Port={options.Port};" +
                    $"Database={options.DatabaseName};" +
                    $"User ID={options.Username};" +
                    $"Password={options.Password};" +
                    "Ssl Mode=Require;",
                    o => o.MigrationsHistoryTable(
                        HistoryRepository.DefaultTableName,
                        "users"
                    )
                );
            });

            services.AddIdentity<UserEntity, IdentityRole<int>>()
                .AddEntityFrameworkStores<UsersDbContext>();
        }

        public void AddDevDbContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("identityDb")!;

            services.AddDbContext<UsersDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseNpgsql(
                    connectionString,
                    o => o.MigrationsHistoryTable(
                        HistoryRepository.DefaultTableName,
                        "users"
                    ));
            });

            services.AddIdentity<UserEntity, IdentityRole<int>>()
                .AddEntityFrameworkStores<UsersDbContext>();
        }

        public void AddExceptionHandlers()
        {
            services.AddExceptionHandler<ValidationExceptionHandler>();

            services.AddProblemDetails();
        }
    }
}