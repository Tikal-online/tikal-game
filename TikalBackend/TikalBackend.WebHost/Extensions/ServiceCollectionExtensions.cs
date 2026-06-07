using Accounts.Application;
using Accounts.Infrastructure;
using FluentValidation;
using Lobbies.Infrastructure;
using MediatR;
using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Shared.Application;
using SignalRApi.Hubs;
using TikalBackend.WebHost.Configuration;
using TikalBackend.WebHost.ExceptionHandlers;
using TikalBackend.WebHost.Pipelines;

namespace TikalBackend.WebHost.Extensions;

internal static class ServiceCollectionExtensions
{
    private static string GetConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("backendDb");

        if (!string.IsNullOrEmpty(connectionString))
        {
            return connectionString;
        }

        var options =
            configuration.GetSection(DatabaseConfiguration.Section).Get<DatabaseConfiguration>()
            ?? throw new InvalidOperationException("Database Configuration is required");

        connectionString = $"Server={options.Host};" +
                           $"Port={options.Port};" +
                           $"Database={options.DatabaseName};" +
                           $"User ID={options.Username};" +
                           $"Password={options.Password};" +
                           "Ssl Mode=Require;";

        return connectionString;
    }

    extension(IServiceCollection services)
    {
        public void ConfigureOpenTelemetry()
        {
            services.AddOpenTelemetry()
                .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddNpgsql()
                        .AddOtlpExporter();
                })
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddNpgsqlInstrumentation()
                        .AddOtlpExporter();
                })
                .WithLogging(logging =>
                {
                    logging
                        .AddOtlpExporter();
                });
        }

        private void AddMediatR()
        {
            services.AddMediatR(c =>
            {
                c.RegisterServicesFromAssemblies(
                    AssemblyReference.Assembly,
                    Lobbies.Application.AssemblyReference.Assembly
                );

                c.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationPipeline<,>));
            });
        }

        private void AddValidators()
        {
            services.AddValidatorsFromAssemblies([
                AssemblyReference.Assembly, Lobbies.Application.AssemblyReference.Assembly
            ]);
        }

        public void AddApplication()
        {
            services.AddValidators();

            services.AddMediatR();

            services.AddSignalRModule();

            services.AddSharedApplication();
        }

        public void AddInfrastructure(IConfiguration configuration)
        {
            var connectionString = GetConnectionString(configuration);

            services.AddAccountsInfrastructure(connectionString);

            services.AddLobbiesInfrastructure(connectionString);
        }

        public void AddExceptionHandlers()
        {
            services.AddExceptionHandler<ValidationExceptionHandler>();

            services.AddProblemDetails();
        }

        public void ConfigureAuthentication(IConfiguration configuration)
        {
            var identityConfiguration = configuration
                .GetRequiredSection(IdentityConfiguration.Position);

            services.Configure<IdentityConfiguration>(identityConfiguration);

            services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.Authority = identityConfiguration.GetValue<string>("Authority");
                    options.TokenValidationParameters.ValidateAudience = false;
                });
        }
    }
}