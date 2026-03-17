using FluentValidation;
using Identity.WebHost.Configuration;
using Identity.WebHost.ExceptionHandlers;
using Identity.WebHost.Pipelines;
using MediatR;
using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Users.Application;
using Users.Infrastructure;

namespace Identity.WebHost.Extensions;

internal static class ServiceCollectionExtensions
{
    private static string GetConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("identityDb");

        if (!string.IsNullOrEmpty(connectionString))
        {
            return connectionString;
        }

        var options = new DatabaseConfiguration();
        configuration.GetSection(DatabaseConfiguration.Section).Bind(options);

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
                        .AddMeter("Identity.*")
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

        public void AddInfrastructure(IConfiguration configuration)
        {
            var connectionString = GetConnectionString(configuration);

            services.AddUsersInfrastructure(connectionString);
        }

        public void AddApplication(IConfiguration configuration)
        {
            services.AddUsersApplication(configuration);
        }

        public void AddExceptionHandlers()
        {
            services.AddExceptionHandler<ValidationExceptionHandler>();

            services.AddProblemDetails();
        }
    }
}