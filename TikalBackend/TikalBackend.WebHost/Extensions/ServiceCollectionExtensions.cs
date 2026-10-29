using System.Diagnostics;
using Accounts.Application;
using Accounts.Infrastructure;
using FluentValidation;
using Games.Infrastructure;
using Humanizer;
using Lobbies.Infrastructure;
using MediatR;
using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Shared.Api;
using TikalBackend.WebHost.Configuration;
using TikalBackend.WebHost.ExceptionHandlers;
using TikalBackend.WebHost.Pipelines;
using TikalBackend.WebHost.Telemetry;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.ErrorHandling;
using Wolverine.Postgresql;

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
                        .AddSource(ActivitySources.ControllerSourceName)
                        .AddSource(ActivitySources.MediatRSourceName)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddNpgsql()
                        .AddSource("Wolverine")
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
                    Lobbies.Application.AssemblyReference.Assembly,
                    Games.Application.AssemblyReference.Assembly,
                    SignalRApi.Hubs.AssemblyReference.Assembly
                );

                c.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TracingPipeline<,>));
                c.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationPipeline<,>));
            });
        }

        private void AddValidators()
        {
            services.AddValidatorsFromAssemblies([
                AssemblyReference.Assembly,
                Lobbies.Application.AssemblyReference.Assembly,
                Games.Application.AssemblyReference.Assembly
            ]);
        }

        public void AddApplication()
        {
            services.AddValidators();

            services.AddMediatR();

            services.AddScoped<AccountContext>();
        }

        public void AddInfrastructure(IConfiguration configuration)
        {
            var connectionString = GetConnectionString(configuration);

            // wolverine runs background jobs which run db queries and do not run under an activity
            // ignore them to make the dashboard cleaner
            var dataSource = new NpgsqlDataSourceBuilder(connectionString)
                .ConfigureTracing(tracing => tracing
                    .ConfigureCommandFilter(_ => Activity.Current is not null)
                    .ConfigureBatchFilter(_ => Activity.Current is not null)
                    .EnablePhysicalOpenTracing(false))
                .Build();

            services.AddWolverine(opts =>
            {
                opts.PersistMessagesWithPostgresql(dataSource, "wolverine");
                opts.UseEntityFrameworkCoreTransactions();

                opts.Policies.AutoApplyTransactions();
                opts.Policies.UseDurableLocalQueues();

                opts.MultipleHandlerBehavior = MultipleHandlerBehavior.Separated;
                opts.Durability.Mode = DurabilityMode.Solo;
                opts.Durability.MessageIdentity = MessageIdentity.IdAndDestination;

                opts.Durability.DurabilityMetricsEnabled = false;

                opts.OnException<Exception>()
                    .RetryWithCooldown(100.Milliseconds(), 500.Milliseconds(), 1.Seconds())
                    .Then.MoveToErrorQueue();
            });

            services.AddAccountsInfrastructure(connectionString);

            services.AddLobbiesInfrastructure(connectionString);

            services.AddGamesInfrastructure(connectionString);
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